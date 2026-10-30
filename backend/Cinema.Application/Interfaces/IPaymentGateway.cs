namespace Cinema.Application.Interfaces;

public record CreatePaymentRequest(
    int BookingId,
    decimal Amount,
    string OrderInfo,
    string TransactionId
);

public record CreatePaymentResult(string PaymentUrl, string? TransactionId);

public record RefundRequest(
    string OriginalTransactionId,
    decimal Amount,
    string OrderInfo,
    DateTime OriginalPaymentDate,
    string? GatewayTransactionId = null
);

public record RefundResult(bool Success, string? RefundTransactionId, string? Message);

public record GatewayCallbackResult(
    bool SignatureValid,
    bool IsSuccess,
    string TransactionId,
    string GatewayResponseCode,
    decimal Amount,
    int BookingId,
    string? GatewayTransactionId = null,
    string? Currency = null
);

public interface IPaymentGateway
{
    string MethodName { get; }

    Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default
    );

    GatewayCallbackResult VerifyCallback(IReadOnlyDictionary<string, string> queryOrFormParams);

    GatewayCallbackResult VerifyWebhook(string rawBody, string? signatureHeader) =>
        new(false, false, string.Empty, string.Empty, 0m, 0);

    Task<RefundResult> RefundAsync(
        RefundRequest request,
        CancellationToken cancellationToken = default
    );
}
