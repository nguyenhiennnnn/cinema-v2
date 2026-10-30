using Cinema.Application.BackgroundJobs;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Payments;

public class PaymentService(
    IAppDbContext db,
    IBookingService bookingService,
    IEnumerable<IPaymentGateway> paymentGateways,
    IBackgroundJobClient backgroundJobClient,
    ILogger<PaymentService> logger,
    IOptions<BrandOptions>? brandOptions = null,
    IOptions<StripeOptions>? stripeOptions = null
) : IPaymentService
{
    private readonly BrandOptions _brand = brandOptions?.Value ?? new();
    private readonly string _stripeCurrency = (stripeOptions?.Value ?? new()).Currency;

    public async Task<VnPayIpnResult> ProcessVnPayIpnAsync(
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken
    )
    {
        var gateway = paymentGateways.FindByMethod(PaymentMethodNames.VnPay);
        if (gateway is null)
        {
            logger.LogWarning("PaymentWebhookReceived VNPay nhưng gateway chưa đăng ký trong DI.");
            return new VnPayIpnResult("99", "Gateway not configured");
        }

        var callback = gateway.VerifyCallback(parameters);
        logger.LogInformation(
            "PaymentWebhookReceived Method VNPay TxnRef {TxnRef} ResponseCode {Code}",
            callback.TransactionId,
            callback.GatewayResponseCode
        );

        if (!callback.SignatureValid)
        {
            logger.LogWarning(
                "PaymentWebhookReceived VNPay chữ ký không hợp lệ — TxnRef {TxnRef}",
                callback.TransactionId
            );
            return new VnPayIpnResult("97", "Invalid signature");
        }

        var payment = await db.Payments.FirstOrDefaultAsync(
            p => p.TransactionId == callback.TransactionId,
            cancellationToken
        );
        if (payment is null)
            return new VnPayIpnResult("01", "Order not found");

        if (IsAlreadyProcessed(payment, callback.IsSuccess))
        {
            logger.LogInformation(
                "PaymentAlreadyProcessed PaymentId {PaymentId} Status {Status}",
                payment.PaymentId,
                payment.Status
            );
            return new VnPayIpnResult("00", "Confirm Success");
        }

        if (Math.Abs(payment.Amount - callback.Amount) > 0.01m)
        {
            logger.LogWarning(
                "PaymentWebhookReceived VNPay sai số tiền — PaymentId {PaymentId} Expected {Expected} Got {Got}",
                payment.PaymentId,
                payment.Amount,
                callback.Amount
            );
            return new VnPayIpnResult("04", "Invalid amount");
        }

        await ApplyPaymentResultAsync(
            payment.PaymentId,
            callback.IsSuccess,
            callback.GatewayResponseCode,
            callback.GatewayTransactionId,
            cancellationToken
        );
        return new VnPayIpnResult("00", "Confirm Success");
    }

    public async Task<MomoIpnResult> ProcessMomoIpnAsync(
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken
    )
    {
        var gateway = paymentGateways.FindByMethod(PaymentMethodNames.Momo);
        if (gateway is null)
        {
            logger.LogWarning("PaymentWebhookReceived MoMo nhưng gateway chưa đăng ký trong DI.");
            return new MomoIpnResult(99, "Gateway not configured");
        }

        var callback = gateway.VerifyCallback(parameters);
        logger.LogInformation(
            "PaymentWebhookReceived Method Momo OrderId {OrderId} ResultCode {Code}",
            callback.TransactionId,
            callback.GatewayResponseCode
        );

        if (!callback.SignatureValid)
        {
            logger.LogWarning(
                "PaymentWebhookReceived MoMo chữ ký không hợp lệ — OrderId {OrderId}",
                callback.TransactionId
            );
            return new MomoIpnResult(97, "Invalid signature");
        }

        var payment = await db.Payments.FirstOrDefaultAsync(
            p => p.TransactionId == callback.TransactionId,
            cancellationToken
        );
        if (payment is null)
            return new MomoIpnResult(1, "Order not found");

        if (IsAlreadyProcessed(payment, callback.IsSuccess))
        {
            logger.LogInformation(
                "PaymentAlreadyProcessed PaymentId {PaymentId} Status {Status}",
                payment.PaymentId,
                payment.Status
            );
            return new MomoIpnResult(0, "success");
        }

        if (Math.Abs(payment.Amount - callback.Amount) > 0.01m)
            return new MomoIpnResult(4, "Invalid amount");

        await ApplyPaymentResultAsync(
            payment.PaymentId,
            callback.IsSuccess,
            callback.GatewayResponseCode,
            callback.GatewayTransactionId,
            cancellationToken
        );
        return new MomoIpnResult(0, "success");
    }

    public async Task<StripeWebhookResult> ProcessStripeWebhookAsync(
        string jsonPayload,
        string? stripeSignature,
        CancellationToken cancellationToken
    )
    {
        var gateway = paymentGateways.FindByMethod(PaymentMethodNames.Stripe);
        if (gateway is null)
        {
            logger.LogWarning("PaymentWebhookReceived Stripe nhưng gateway chưa đăng ký trong DI.");
            return new StripeWebhookResult(false, "Gateway not configured");
        }

        var callback = gateway.VerifyWebhook(jsonPayload, stripeSignature);
        logger.LogInformation(
            "PaymentWebhookReceived Method Stripe TxnRef {TxnRef} Success {IsSuccess}",
            callback.TransactionId,
            callback.IsSuccess
        );

        if (!callback.SignatureValid)
        {
            logger.LogWarning("PaymentWebhookReceived Stripe chữ ký không hợp lệ.");
            return new StripeWebhookResult(false, "Invalid signature");
        }

        if (string.IsNullOrEmpty(callback.TransactionId))
        {
            return new StripeWebhookResult(true, "Event ignored");
        }

        var payment = await db.Payments.FirstOrDefaultAsync(
            p => p.TransactionId == callback.TransactionId,
            cancellationToken
        );
        if (payment is null)
        {
            logger.LogWarning(
                "PaymentWebhookReceived Stripe Payment not found TxnRef {TxnRef}",
                callback.TransactionId
            );
            return new StripeWebhookResult(false, "Payment not found");
        }

        if (IsAlreadyProcessed(payment, callback.IsSuccess))
        {
            logger.LogInformation(
                "PaymentAlreadyProcessed PaymentId {PaymentId} Status {Status}",
                payment.PaymentId,
                payment.Status
            );
            return new StripeWebhookResult(true, "Payment already processed");
        }

        if (Math.Abs(payment.Amount - callback.Amount) > 0.01m)
        {
            logger.LogWarning(
                "PaymentWebhookReceived Stripe sai số tiền — PaymentId {PaymentId} Expected {Expected} Got {Got}",
                payment.PaymentId,
                payment.Amount,
                callback.Amount
            );
            return new StripeWebhookResult(false, "Invalid amount");
        }

        if (
            !string.IsNullOrWhiteSpace(callback.Currency)
            && !string.Equals(callback.Currency, _stripeCurrency, StringComparison.OrdinalIgnoreCase)
        )
        {
            logger.LogWarning(
                "PaymentWebhookReceived Stripe sai currency — PaymentId {PaymentId} Expected {Expected} Got {Got}",
                payment.PaymentId,
                _stripeCurrency,
                callback.Currency
            );
            return new StripeWebhookResult(false, "Invalid currency");
        }

        await ApplyPaymentResultAsync(
            payment.PaymentId,
            callback.IsSuccess,
            callback.GatewayResponseCode,
            callback.GatewayTransactionId,
            cancellationToken
        );
        return new StripeWebhookResult(true, "Payment processed successfully");
    }

    private async Task ApplyPaymentResultAsync(
        int paymentId,
        bool isSuccess,
        string gatewayResponseCode,
        string? gatewayTransactionId,
        CancellationToken cancellationToken
    )
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var payment = await db.Payments.FirstAsync(
            p => p.PaymentId == paymentId,
            cancellationToken
        );

        payment.GatewayResponseCode = gatewayResponseCode;
        payment.Status = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
        if (isSuccess)
        {
            payment.PaidAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(gatewayTransactionId))
                payment.GatewayTransactionId = gatewayTransactionId;
        }
        await db.SaveChangesAsync(cancellationToken);

        if (!isSuccess)
        {
            await transaction.CommitAsync(cancellationToken);
            await bookingService.ProcessPaymentFailureAsync(payment.BookingId, cancellationToken);
            logger.LogInformation(
                "PaymentFailed PaymentId {PaymentId} BookingId {BookingId}",
                payment.PaymentId,
                payment.BookingId
            );
            return;
        }

        try
        {
            await bookingService.ConfirmBookingAndGenerateTicketsAsync(
                payment.BookingId,
                cancellationToken
            );
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation(
                "PaymentVerified PaymentId {PaymentId} BookingId {BookingId}",
                payment.PaymentId,
                payment.BookingId
            );
        }
        catch (SeatsNoLongerHeldException)
        {
            await transaction.CommitAsync(cancellationToken);
            await HandleSeatsExpiredAfterPaymentAsync(payment, cancellationToken);
        }
    }

    private static bool IsAlreadyProcessed(Payment payment, bool callbackSuccess) =>
        payment.Status is PaymentStatus.Success or PaymentStatus.Refunded
        || (payment.Status == PaymentStatus.Failed && !callbackSuccess);

    private async Task HandleSeatsExpiredAfterPaymentAsync(
        Payment payment,
        CancellationToken cancellationToken
    )
    {
        logger.LogCritical(
            "SeatsExpiredAfterPayment PaymentId {PaymentId} BookingId {BookingId} Amount {Amount} — đang thử hoàn tiền tự động.",
            payment.PaymentId,
            payment.BookingId,
            payment.Amount
        );

        var gateway = paymentGateways.FindByMethod(payment.PaymentMethod.ToString());

        RefundResult? refundResult = null;
        if (gateway is not null && payment.TransactionId is not null)
        {
            try
            {
                refundResult = await gateway.RefundAsync(
                    new RefundRequest(
                        payment.TransactionId,
                        payment.Amount,
                        $"Hoàn tiền tự động - ghế hết hạn giữ - Payment {payment.PaymentId}",
                        payment.PaidAt ?? payment.CreatedAt,
                        GatewayTransactionId: payment.GatewayTransactionId
                    ),
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "AutoRefundFailed PaymentId {PaymentId} — lỗi khi gọi gateway hoàn tiền.",
                    payment.PaymentId
                );
            }
        }

        var autoRefunded = refundResult is { Success: true };
        if (autoRefunded)
        {
            payment.Status = PaymentStatus.Refunded;
            payment.RefundedAmount = payment.Amount;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundTransactionId = refundResult!.RefundTransactionId;
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "AutoRefundSucceeded PaymentId {PaymentId} BookingId {BookingId}",
                payment.PaymentId,
                payment.BookingId
            );
        }

        var settings = await db.CinemaSettings.FirstOrDefaultAsync(
            s => s.Id == 1,
            cancellationToken
        );
        if (!string.IsNullOrWhiteSpace(settings?.Email))
        {
            var html = EmailTemplates.SeatsExpiredAfterPayment(
                _brand,
                autoRefunded,
                payment.BookingId,
                payment.PaymentId,
                payment.Amount
            );
            backgroundJobClient.Enqueue<SendEmailJob>(j =>
                j.RunAsync(
                    settings.Email!,
                    autoRefunded
                        ? $"[{_brand.Name}] Đã tự động hoàn tiền — Booking #{payment.BookingId}"
                        : $"[{_brand.Name}] CẦN HOÀN TIỀN THỦ CÔNG — Booking #{payment.BookingId}",
                    html,
                    CancellationToken.None
                )
            );
        }

        var customer = await db
            .Bookings.Where(b => b.BookingId == payment.BookingId && b.UserId != null)
            .Select(b => new
            {
                b.BookingCode,
                b.CustomerName,
                b.User!.Email,
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (customer is not null && !string.IsNullOrWhiteSpace(customer.Email))
        {
            var customerHtml = EmailTemplates.SeatsExpiredAfterPaymentCustomer(
                _brand,
                customer.CustomerName,
                customer.BookingCode,
                autoRefunded,
                payment.Amount
            );
            backgroundJobClient.Enqueue<SendEmailJob>(j =>
                j.RunAsync(
                    customer.Email,
                    autoRefunded
                        ? $"[{_brand.Name}] Đã hoàn tiền — Booking {customer.BookingCode}"
                        : $"[{_brand.Name}] Chờ hoàn tiền — Booking {customer.BookingCode}",
                    customerHtml,
                    CancellationToken.None
                )
            );
        }
    }
}
