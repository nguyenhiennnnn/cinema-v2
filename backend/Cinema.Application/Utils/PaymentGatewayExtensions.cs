using Cinema.Application.Interfaces;

namespace Cinema.Application.Utils;

public static class PaymentGatewayExtensions
{
    public static IPaymentGateway? FindByMethod(
        this IEnumerable<IPaymentGateway> gateways,
        string methodName
    ) => gateways.FirstOrDefault(g => g.MethodName == methodName);
}
