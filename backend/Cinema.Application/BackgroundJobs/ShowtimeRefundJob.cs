using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.BackgroundJobs;

public class ShowtimeRefundJob(
    IAppDbContext db,
    IEnumerable<IPaymentGateway> paymentGateways,
    IBackgroundJobClient backgroundJobClient,
    ILogger<ShowtimeRefundJob> logger,
    IOptions<BrandOptions>? brandOptions = null
)
{
    private readonly BrandOptions _brand = brandOptions?.Value ?? new();

    public async Task RunAsync(int paymentId, CancellationToken cancellationToken)
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
        var gateway = paymentGateways.FirstOrDefault(g =>
            g.MethodName == payment.PaymentMethod.ToString()
        );

        var refundResult = gateway is null
            ? new RefundResult(false, null, "Cổng thanh toán chưa được cấu hình.")
            : await gateway.RefundAsync(
                new RefundRequest(
                    payment.TransactionId!,
                    payment.Amount,
                    $"Hoàn tiền do rạp hủy suất chiếu - booking {booking.BookingCode}",
                    payment.PaidAt ?? payment.CreatedAt,
                    GatewayTransactionId: payment.GatewayTransactionId
                ),
                cancellationToken
            );

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
                EnqueueEmail(
                    booking.User.Email,
                    $"Suất chiếu bị hủy — đã hoàn tiền {booking.BookingCode}",
                    EmailTemplates.ShowtimeCancelledAutoRefunded(
                        _brand,
                        booking.CustomerName,
                        booking.BookingCode,
                        payment.Amount
                    )
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

        if (booking.User is not null)
            EnqueueEmail(
                booking.User.Email,
                $"Suất chiếu bạn đặt vé đã bị hủy — {booking.BookingCode}",
                EmailTemplates.ShowtimeCancelledManualRefund(
                    _brand,
                    booking.CustomerName,
                    booking.BookingCode,
                    payment.Amount
                )
            );
    }

    private void EnqueueEmail(string toEmail, string subject, string html) =>
        backgroundJobClient.Enqueue<SendEmailJob>(j =>
            j.RunAsync(toEmail, subject, html, CancellationToken.None)
        );
}
