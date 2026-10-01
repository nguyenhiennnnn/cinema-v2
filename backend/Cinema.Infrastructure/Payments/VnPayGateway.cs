using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Shared.Constants;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.Payments;

public class VnPayGateway(IOptions<VnPayOptions> options, HttpClient httpClient) : IPaymentGateway
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly VnPayOptions _options = options.Value;

    public string MethodName => PaymentMethodNames.VnPay;

    private static readonly TimeZoneInfo VietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
        OperatingSystem.IsWindows() ? "SE Asia Standard Time" : "Asia/Ho_Chi_Minh"
    );

    private static DateTime NowVietnam() =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, VietnamTimeZone);

    public Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var returnUrl = $"{_options.ReturnUrl}?bookingId={request.BookingId}";

        var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = _options.TmnCode,
            ["vnp_Amount"] = ((long)(request.Amount * 100)).ToString(),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = request.TransactionId,
            ["vnp_OrderInfo"] = request.OrderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = returnUrl,
            ["vnp_IpAddr"] = "127.0.0.1",
            ["vnp_CreateDate"] = NowVietnam().ToString("yyyyMMddHHmmss"),
        };

        var signData = BuildSignData(vnpParams);
        var secureHash = HmacSha512(_options.HashSecret, signData);

        var query = string.Join(
            "&",
            vnpParams.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}")
        );
        var paymentUrl = $"{_options.BaseUrl}?{query}&vnp_SecureHash={secureHash}";

        return Task.FromResult(new CreatePaymentResult(paymentUrl, request.TransactionId));
    }

    public GatewayCallbackResult VerifyCallback(
        IReadOnlyDictionary<string, string> queryOrFormParams
    )
    {
        var receivedHash = queryOrFormParams.GetValueOrDefault("vnp_SecureHash", string.Empty);

        var paramsForSigning = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var kv in queryOrFormParams)
        {
            if (kv.Key is "vnp_SecureHash" or "vnp_SecureHashType")
                continue;
            if (string.IsNullOrEmpty(kv.Value))
                continue;
            paramsForSigning[kv.Key] = kv.Value;
        }

        var signData = BuildSignData(paramsForSigning);
        var computedHash = HmacSha512(_options.HashSecret, signData);
        var signatureValid = string.Equals(
            computedHash,
            receivedHash,
            StringComparison.OrdinalIgnoreCase
        );

        var responseCode = queryOrFormParams.GetValueOrDefault("vnp_ResponseCode", string.Empty);
        var amountRaw = queryOrFormParams.GetValueOrDefault("vnp_Amount", "0");
        var amount = long.TryParse(amountRaw, out var amountLong) ? amountLong / 100m : 0m;
        var txnRef = queryOrFormParams.GetValueOrDefault("vnp_TxnRef", string.Empty);
        var vnpTransactionNo = queryOrFormParams.GetValueOrDefault(
            "vnp_TransactionNo",
            string.Empty
        );

        return new GatewayCallbackResult(
            signatureValid,
            responseCode == "00",
            txnRef,
            responseCode,
            amount,
            BookingId: 0,
            GatewayTransactionId: string.IsNullOrEmpty(vnpTransactionNo) ? null : vnpTransactionNo
        );
    }

    public async Task<RefundResult> RefundAsync(
        RefundRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var requestId = Guid.NewGuid().ToString("N")[..16];
        var createDate = NowVietnam().ToString("yyyyMMddHHmmss");
        var vnpTxnRef = request.OriginalTransactionId;
        var vnpAmount = ((long)(request.Amount * 100)).ToString();
        var originalPaymentDateVn = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(request.OriginalPaymentDate, DateTimeKind.Utc),
            VietnamTimeZone
        );
        var vnpTransactionDate = originalPaymentDateVn.ToString("yyyyMMddHHmmss");
        var vnpTransactionNo = request.GatewayTransactionId ?? string.Empty;
        const string transactionType = "02";
        const string createBy = "System";
        const string ipAddr = "127.0.0.1";

        var signData = string.Join(
            "|",
            requestId,
            "2.1.0",
            "refund",
            _options.TmnCode,
            transactionType,
            vnpTxnRef,
            vnpAmount,
            vnpTransactionNo,
            vnpTransactionDate,
            createBy,
            createDate,
            ipAddr,
            request.OrderInfo
        );
        var secureHash = HmacSha512(_options.HashSecret, signData);

        var payload = new
        {
            vnp_RequestId = requestId,
            vnp_Version = "2.1.0",
            vnp_Command = "refund",
            vnp_TmnCode = _options.TmnCode,
            vnp_TransactionType = transactionType,
            vnp_TxnRef = vnpTxnRef,
            vnp_Amount = vnpAmount,
            vnp_TransactionNo = vnpTransactionNo,
            vnp_TransactionDate = vnpTransactionDate,
            vnp_CreateBy = createBy,
            vnp_CreateDate = createDate,
            vnp_IpAddr = ipAddr,
            vnp_OrderInfo = request.OrderInfo,
            vnp_SecureHash = secureHash,
        };

        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                _options.ApiUrl,
                payload,
                cancellationToken
            );
            var result = await response.Content.ReadFromJsonAsync<VnPayRefundResponse>(
                JsonOptions,
                cancellationToken
            );

            if (result is null)
                return new RefundResult(false, null, "VNPay trả về response rỗng/không hợp lệ.");

            return result.vnp_ResponseCode == "00"
                ? new RefundResult(true, result.vnp_TransactionNo, null)
                : new RefundResult(
                    false,
                    null,
                    $"VNPay ResponseCode={result.vnp_ResponseCode} Message={result.vnp_Message}"
                );
        }
        catch (Exception ex)
        {
            return new RefundResult(false, null, $"Lỗi gọi VNPay refund API: {ex.Message}");
        }
    }

    private record VnPayRefundResponse(
        string vnp_ResponseCode,
        string? vnp_Message,
        string? vnp_TransactionNo
    );

    private static string BuildSignData(SortedDictionary<string, string> sortedParams) =>
        string.Join("&", sortedParams.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));

    private static string HmacSha512(string key, string data)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
