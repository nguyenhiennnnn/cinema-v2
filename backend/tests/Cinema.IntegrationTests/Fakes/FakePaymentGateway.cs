using Cinema.Application.Interfaces;

namespace Cinema.IntegrationTests.Fakes;

public class FakePaymentGateway(string methodName) : IPaymentGateway
{
    public string MethodName { get; } = methodName;

    public CreatePaymentResult NextCreatePaymentResult { get; set; } =
        new("https://fake-gateway.test/pay", "FAKE-TXN");

    public GatewayCallbackResult? NextCallbackResult { get; set; }

    public RefundResult NextRefundResult { get; set; } = new(true, "FAKE-REFUND-TXN", null);

    public List<CreatePaymentRequest> CreatePaymentRequests { get; } = [];
    public List<RefundRequest> RefundRequests { get; } = [];

    public Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default
    )
    {
        CreatePaymentRequests.Add(request);
        return Task.FromResult(NextCreatePaymentResult);
    }

    public GatewayCallbackResult VerifyCallback(
        IReadOnlyDictionary<string, string> queryOrFormParams
    ) =>
        NextCallbackResult
        ?? throw new InvalidOperationException(
            $"FakePaymentGateway({MethodName}).NextCallbackResult was not configured for this test."
        );

    public Task<RefundResult> RefundAsync(
        RefundRequest request,
        CancellationToken cancellationToken = default
    )
    {
        RefundRequests.Add(request);
        return Task.FromResult(NextRefundResult);
    }
}
