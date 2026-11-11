using System.Net.Http.Json;
using System.Text.Json;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.Extensions.Options;

namespace Cinema.Infrastructure.Payments;

public class MomoGateway(IOptions<MomoOptions> options, HttpClient httpClient) : IPaymentGateway
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly MomoOptions _options = options.Value;

    public string MethodName => PaymentMethodNames.Momo;

    public async Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var requestId = Guid.NewGuid().ToString();
        var orderId = request.TransactionId;
        var amount = ((long)request.Amount).ToString();
        const string extraData = "";
        const string requestType = "captureWallet";
        const string orderType = "other";
        var returnUrl = $"{_options.ReturnUrl}?bookingId={request.BookingId}";

        var rawSignature =
            $"accessKey={_options.AccessKey}&amount={amount}&extraData={extraData}"
            + $"&ipnUrl={_options.IpnUrl}&orderId={orderId}&orderInfo={request.OrderInfo}"
            + $"&partnerCode={_options.PartnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType={requestType}";

        var signature = HmacSigner.Sha256Hex(_options.SecretKey, rawSignature);

        var payload = new
        {
            partnerCode = _options.PartnerCode,
            requestId,
            amount,
            orderId,
            orderInfo = request.OrderInfo,
            redirectUrl = returnUrl,
            ipnUrl = _options.IpnUrl,
            extraData,
            requestType,
            orderType,
            signature,
            lang = "vi",
        };

        using var response = await httpClient.PostAsJsonAsync(
            _options.Endpoint,
            payload,
            cancellationToken
        );
        var result =
            await response.Content.ReadFromJsonAsync<MomoCreateResponse>(
                JsonOptions,
                cancellationToken
            ) ?? throw new BusinessRuleException(Messages.Rule_MomoInvalidResponse);

        if (result.ResultCode != 0)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_MomoCreatePaymentFailed, result.Message)
            );

        return new CreatePaymentResult(result.PayUrl ?? string.Empty, orderId);
    }

    public GatewayCallbackResult VerifyCallback(
        IReadOnlyDictionary<string, string> queryOrFormParams
    )
    {
        var receivedSignature = queryOrFormParams.GetValueOrDefault("signature", string.Empty);

        var rawSignature =
            $"accessKey={_options.AccessKey}&amount={queryOrFormParams.GetValueOrDefault("amount")}"
            + $"&extraData={queryOrFormParams.GetValueOrDefault("extraData")}&message={queryOrFormParams.GetValueOrDefault("message")}"
            + $"&orderId={queryOrFormParams.GetValueOrDefault("orderId")}&orderInfo={queryOrFormParams.GetValueOrDefault("orderInfo")}"
            + $"&orderType={queryOrFormParams.GetValueOrDefault("orderType")}&partnerCode={queryOrFormParams.GetValueOrDefault("partnerCode")}"
            + $"&payType={queryOrFormParams.GetValueOrDefault("payType")}&requestId={queryOrFormParams.GetValueOrDefault("requestId")}"
            + $"&responseTime={queryOrFormParams.GetValueOrDefault("responseTime")}&resultCode={queryOrFormParams.GetValueOrDefault("resultCode")}"
            + $"&transId={queryOrFormParams.GetValueOrDefault("transId")}";

        var computedSignature = HmacSigner.Sha256Hex(_options.SecretKey, rawSignature);
        var signatureValid = string.Equals(
            computedSignature,
            receivedSignature,
            StringComparison.OrdinalIgnoreCase
        );

        var resultCode = queryOrFormParams.GetValueOrDefault("resultCode", "-1");
        var amount = decimal.TryParse(queryOrFormParams.GetValueOrDefault("amount"), out var amt)
            ? amt
            : 0m;
        var orderId = queryOrFormParams.GetValueOrDefault("orderId", string.Empty);
        var momoTransId = queryOrFormParams.GetValueOrDefault("transId", string.Empty);

        return new GatewayCallbackResult(
            signatureValid,
            resultCode == "0",
            orderId,
            resultCode,
            amount,
            BookingId: 0,
            GatewayTransactionId: string.IsNullOrEmpty(momoTransId) ? null : momoTransId
        );
    }

    public async Task<RefundResult> RefundAsync(
        RefundRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var requestId = Guid.NewGuid().ToString();
        var orderId = Guid.NewGuid().ToString();
        var amount = ((long)request.Amount).ToString();

        var transId = request.GatewayTransactionId ?? request.OriginalTransactionId;

        var rawSignature =
            $"accessKey={_options.AccessKey}&amount={amount}&description={request.OrderInfo}"
            + $"&orderId={orderId}&partnerCode={_options.PartnerCode}&requestId={requestId}&transId={transId}";
        var signature = HmacSigner.Sha256Hex(_options.SecretKey, rawSignature);

        var payload = new
        {
            partnerCode = _options.PartnerCode,
            orderId,
            requestId,
            amount,
            transId,
            lang = "vi",
            description = request.OrderInfo,
            signature,
        };

        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                _options.RefundEndpoint,
                payload,
                cancellationToken
            );
            var result = await response.Content.ReadFromJsonAsync<MomoRefundResponse>(
                JsonOptions,
                cancellationToken
            );

            if (result is null)
                return new RefundResult(false, null, "MoMo trả về response rỗng/không hợp lệ.");

            return result.ResultCode == 0
                ? new RefundResult(true, orderId, null)
                : new RefundResult(
                    false,
                    null,
                    $"MoMo hoàn tiền thất bại: ResultCode={result.ResultCode} Message={result.Message}"
                );
        }
        catch (Exception ex)
        {
            return new RefundResult(false, null, $"Lỗi gọi MoMo refund API: {ex.Message}");
        }
    }

    private record MomoCreateResponse(int ResultCode, string? Message, string? PayUrl);

    private record MomoRefundResponse(int ResultCode, string? Message);
}
