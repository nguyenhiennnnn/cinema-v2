using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Showtimes;

public class ShowtimeCancellationService(
    IAppDbContext db,
    IBookingFulfillmentService bookingFulfillmentService,
    INotificationService notificationService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<ShowtimeCancellationService> logger
) : IShowtimeCancellationService
{
    public async Task<DeleteShowtimeResponse> DeleteShowtimeAsync(
        int showtimeId,
        CancellationToken cancellationToken
    )
    {
        var showtime =
            await db.Showtimes.FirstOrDefaultAsync(
                s => s.ShowtimeId == showtimeId,
                cancellationToken
            ) ?? throw new NotFoundException(Messages.NotFound_Showtime);

        ShowtimeRules.EnsureScheduled(showtime, Messages.Common_ActionCancel);
        if (showtime.StartTime <= DateTime.UtcNow)
            throw new BusinessRuleException(Messages.Rule_ShowtimeStartedCannotCancel);

        var manualRefundCount = 0;
        var refundPaymentIds = new List<int>();
        var manualRefundNotices = new List<(string ToEmail, string CustomerName, string BookingCode, decimal Amount)>();
        var bookingCount = 0;

        await using (var transaction = await db.Database.BeginTransactionAsync(cancellationToken))
        {
            showtime.Status = ShowtimeStatus.Cancelled;

            var bookings = await db
                .Bookings.Include(b => b.Payments)
                .Include(b => b.User)
                .Where(b =>
                    b.ShowtimeId == showtimeId
                    && (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed)
                )
                .ToListAsync(cancellationToken);
            bookingCount = bookings.Count;

            foreach (var booking in bookings)
            {
                var payment = booking
                    .Payments.Where(p => p.Status == PaymentStatus.Success)
                    .OrderByDescending(p => p.PaymentId)
                    .FirstOrDefault();

                var wasConfirmed = booking.Status == BookingStatus.Confirmed;
                booking.Status = BookingStatus.Cancelled;

                if (!wasConfirmed)
                    foreach (
                        var pending in booking.Payments.Where(p => p.Status == PaymentStatus.Pending)
                    )
                        pending.Status = PaymentStatus.Failed;

                if (wasConfirmed && booking.UserId is not null)
                    await bookingFulfillmentService.ReverseLoyaltyAsync(
                        booking.UserId.Value,
                        booking.BookingId,
                        booking.FinalAmount,
                        cancellationToken
                    );

                if (payment is null)
                    continue;

                if (payment.PaymentMethod == PaymentMethod.Cash)
                {
                    manualRefundCount++;
                    if (booking.User is not null)
                        manualRefundNotices.Add(
                            (
                                booking.User.Email,
                                booking.CustomerName,
                                booking.BookingCode,
                                payment.Amount
                            )
                        );
                    continue;
                }

                refundPaymentIds.Add(payment.PaymentId);
            }

            await db
                .ShowtimeSeats.Where(ss =>
                    ss.ShowtimeId == showtimeId && ss.Status == ShowtimeSeatStatus.Held
                )
                .ExecuteUpdateAsync(
                    s =>
                        s.SetProperty(x => x.Status, ShowtimeSeatStatus.Available)
                            .SetProperty(x => x.HeldByUserId, (int?)null)
                            .SetProperty(x => x.HeldByBookingId, (int?)null)
                            .SetProperty(x => x.HeldUntil, (DateTime?)null),
                    cancellationToken
                );

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        foreach (var notice in manualRefundNotices)
        {
            try
            {
                notificationService.SendShowtimeCancelledManualRefund(
                    notice.ToEmail,
                    notice.CustomerName,
                    notice.BookingCode,
                    notice.Amount
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "ShowtimeCancelEmailEnqueueFailed ShowtimeId {ShowtimeId}",
                    showtimeId
                );
            }
        }

        foreach (var paymentId in refundPaymentIds)
        {
            try
            {
                backgroundJobClient.Enqueue<ShowtimeRefundJob>(j =>
                    j.RunAsync(paymentId, null, CancellationToken.None)
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "ShowtimeRefundEnqueueFailed ShowtimeId {ShowtimeId} PaymentId {PaymentId}",
                    showtimeId,
                    paymentId
                );
            }
        }

        logger.LogInformation(
            "ShowtimeCancelled ShowtimeId {ShowtimeId} BookingsCancelled {BookingCount} AutoRefundQueued {QueuedCount} ManualRefundNeeded {ManualCount}",
            showtimeId,
            bookingCount,
            refundPaymentIds.Count,
            manualRefundCount
        );

        return new DeleteShowtimeResponse(
            showtimeId,
            "Cancelled",
            manualRefundCount,
            refundPaymentIds.Count
        );
    }
}

