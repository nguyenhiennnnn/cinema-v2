namespace Cinema.Application.Interfaces;

public record VnPayIpnResult(string RspCode, string Message);

public record MomoIpnResult(int ResultCode, string Message);

public record StripeWebhookResult(bool Success, string Message);

public interface IPaymentService
{
    Task<VnPayIpnResult> ProcessVnPayIpnAsync(
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken
    );
    Task<MomoIpnResult> ProcessMomoIpnAsync(
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken
    );
    Task<StripeWebhookResult> ProcessStripeWebhookAsync(
        string jsonPayload,
        string? stripeSignature,
        CancellationToken cancellationToken
    );
}
