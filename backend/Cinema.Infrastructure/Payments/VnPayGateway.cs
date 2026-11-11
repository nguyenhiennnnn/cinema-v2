using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Shared.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.Payments;

public class VnPayGateway(
    IOptions<VnPayOptions> options,
    HttpClient httpClient,
    ILogger<VnPayGateway> logger,
    TimeProvider? timeProvider = null
) : IPaymentGateway
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly VnPayOptions _options = options.Value;
    private readonly TimeProvider _time = timeProvider ?? TimeProvider.System;

    public string MethodName => PaymentMethodNames.VnPay;

    private const string ApiVersion = "2.1.0";
    private const string DateFormat = "yyyyMMddHHmmss";
    private const string SuccessResponseCode = "00";
    private const string DefaultIpAddress = "127.0.0.1";
    private const decimal AmountScale = 100m;
    private const string CurrencyCode = "VND";
    private static readonly TimeSpan VnPayOffset = TimeSpan.FromHours(7);

    private static string FormatVnPayDate(DateTime utc) =>
        DateTime
            .SpecifyKind(utc, DateTimeKind.Utc)
            .Add(VnPayOffset)
            .ToString(DateFormat, CultureInfo.InvariantCulture);

    private static string ToVnpAmount(decimal amount) => ((long)(amount * AmountScale)).ToString();

    public Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var returnUrl = $"{_options.ReturnUrl}?bookingId={request.BookingId}";

        var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = ApiVersion,
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = _options.TmnCode,
            ["vnp_Amount"] = ToVnpAmount(request.Amount),
            ["vnp_CurrCode"] = CurrencyCode,
            ["vnp_TxnRef"] = request.TransactionId,
            ["vnp_OrderInfo"] = request.OrderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = returnUrl,
            ["vnp_IpAddr"] = DefaultIpAddress,
            ["vnp_CreateDate"] = FormatVnPayDate(_time.GetUtcNow().UtcDateTime),
        };

        var signData = BuildSignData(vnpParams);
        var secureHash = HmacSigner.Sha512Hex(_options.HashSecret, signData);

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
        var computedHash = HmacSigner.Sha512Hex(_options.HashSecret, signData);
        var signatureValid = string.Equals(
            computedHash,
            receivedHash,
            StringComparison.OrdinalIgnoreCase
        );

        var responseCode = queryOrFormParams.GetValueOrDefault("vnp_ResponseCode", string.Empty);
        var amountRaw = queryOrFormParams.GetValueOrDefault("vnp_Amount", "0");
        var amount = long.TryParse(amountRaw, out var amountLong) ? amountLong / AmountScale : 0m;
        var txnRef = queryOrFormParams.GetValueOrDefault("vnp_TxnRef", string.Empty);
        var vnpTransactionNo = queryOrFormParams.GetValueOrDefault(
            "vnp_TransactionNo",
            string.Empty
        );

        return new GatewayCallbackResult(
            signatureValid,
            responseCode == SuccessResponseCode,
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
        var createDate = FormatVnPayDate(_time.GetUtcNow().UtcDateTime);
        var vnpTxnRef = request.OriginalTransactionId;
        var vnpAmount = ToVnpAmount(request.Amount);
        var vnpTransactionDate = FormatVnPayDate(request.OriginalPaymentDate);
        var vnpTransactionNo = request.GatewayTransactionId ?? string.Empty;
        const string transactionType = "02";
        const string createBy = "System";
        const string ipAddr = DefaultIpAddress;

        var signData = string.Join(
            "|",
            requestId,
            ApiVersion,
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
        var secureHash = HmacSigner.Sha512Hex(_options.HashSecret, signData);

        var payload = new
        {
            vnp_RequestId = requestId,
            vnp_Version = ApiVersion,
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

            return result.vnp_ResponseCode == SuccessResponseCode
                ? new RefundResult(true, result.vnp_TransactionNo, null)
                : new RefundResult(
                    false,
                    null,
                    $"VNPay ResponseCode={result.vnp_ResponseCode} Message={result.vnp_Message}"
                );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "VnPayRefundApiError OriginalTransactionId {TransactionId}", request.OriginalTransactionId);
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
}
