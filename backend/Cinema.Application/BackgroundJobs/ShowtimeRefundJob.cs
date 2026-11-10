using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using Hangfire;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.BackgroundJobs;

public class ShowtimeRefundJob(
    IAppDbContext db,
    IEnumerable<IPaymentGateway> paymentGateways,
    INotificationService notificationService,
    ILogger<ShowtimeRefundJob> logger,
    IOptions<BackgroundJobOptions>? jobOptions = null
)
{
    private readonly int _retryAttempts = (jobOptions?.Value ?? new()).RetryAttempts;

    public async Task RunAsync(
        int paymentId,
        PerformContext? context,
        CancellationToken cancellationToken
    )
    {
        var payment = await db
            .Payments.Include(p => p.Booking)
                .ThenInclude(b => b.User)
            .Include(p => p.Booking)
                .ThenInclude(b => b.Showtime)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken);

        if (payment is null)
        {
            logger.LogWarning("ShowtimeRefundJob: PaymentId {PaymentId} không tồn tại.", paymentId);
            return;
        }

        if (payment.Status != PaymentStatus.Success)
            return;

        if (
            payment.Booking.Status != BookingStatus.Cancelled
            || payment.Booking.Showtime.Status != ShowtimeStatus.Cancelled
        )
        {
            logger.LogWarning(
                "ShowtimeRefundJob bỏ qua PaymentId {PaymentId}: booking/suất chiếu chưa ở trạng thái Cancelled.",
                paymentId
            );
            return;
        }

        if (payment.PaymentMethod == PaymentMethod.Cash)
            return;

        var booking = payment.Booking;
        var gateway = paymentGateways.FindByMethod(payment.PaymentMethod.ToString());

        RefundResult refundResult;
        if (gateway is null)
            refundResult = new RefundResult(
                false,
                null,
                string.Format(Messages.Rule_RefundGatewayNotConfigured, payment.PaymentMethod)
            );
        else
            try
            {
                refundResult = await gateway.RefundAsync(
                    new RefundRequest(
                        payment.TransactionId!,
                        payment.Amount,
                        string.Format(
                            Messages.Refund_ShowtimeCancelledDescription,
                            booking.BookingCode
                        ),
                        payment.PaidAt ?? payment.CreatedAt,
                        GatewayTransactionId: payment.GatewayTransactionId
                    ),
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ShowtimeRefundGatewayError PaymentId {PaymentId}", paymentId);
                refundResult = new RefundResult(false, null, ex.Message);
            }

        if (refundResult.Success)
        {
            payment.Status = PaymentStatus.Refunded;
            payment.RefundedAmount = payment.Amount;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundTransactionId = refundResult.RefundTransactionId;
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "ShowtimeRefunded PaymentId {PaymentId} BookingId {BookingId}",
                payment.PaymentId,
                booking.BookingId
            );

            if (booking.User is not null)
                notificationService.SendShowtimeCancelledRefunded(
                    booking.User.Email,
                    booking.CustomerName,
                    booking.BookingCode,
                    payment.Amount
                );
            return;
        }

        logger.LogWarning(
            "ShowtimeCancelRefundFailed ShowtimeId {ShowtimeId} BookingId {BookingId} PaymentId {PaymentId} Message {Message}",
            booking.ShowtimeId,
            booking.BookingId,
            payment.PaymentId,
            refundResult.Message
        );

        var retryCount = context?.GetJobParameter<int>("RetryCount") ?? _retryAttempts;
        var failureMessage = $"Hoàn tiền thất bại PaymentId {paymentId}: {refundResult.Message}";
        if (retryCount < _retryAttempts)
            throw new InvalidOperationException(failureMessage);

        if (booking.User is not null)
            notificationService.SendShowtimeCancelledManualRefund(
                booking.User.Email,
                booking.CustomerName,
                booking.BookingCode,
                payment.Amount
            );

        throw new InvalidOperationException(failureMessage);
    }
}
