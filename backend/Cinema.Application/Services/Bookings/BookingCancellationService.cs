using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Bookings;

public class BookingCancellationService(
    IAppDbContext db,
    IRealtimeNotifier realtimeNotifier,
    IEnumerable<IPaymentGateway> paymentGateways,
    INotificationService notificationService,
    IBookingFulfillmentService fulfillmentService,
    BookingSeatReleaser releaser,
    ILogger<BookingCancellationService> logger
) : IBookingCancellationService
{
    private record RefundEligibility(bool Eligible, decimal RefundAmount, string? IneligibleReason);

    private static RefundEligibility CalculateRefundEligibility(
        DateTime showtimeStartTime,
        decimal paidAmount,
        CinemaSettings settings
    )
    {
        if (settings.CancelRefundPercent <= 0)
            return new RefundEligibility(false, 0m, Messages.Rule_RefundNotSupported);

        if (DateTime.UtcNow > showtimeStartTime.AddHours(-settings.CancelDeadlineHours))
            return new RefundEligibility(
                false,
                0m,
                string.Format(Messages.Rule_RefundDeadlineExceeded, settings.CancelDeadlineHours)
            );

        var refundAmount = Math.Round(
            paidAmount * settings.CancelRefundPercent / 100m,
            0,
            MidpointRounding.AwayFromZero
        );
        return new RefundEligibility(true, refundAmount, null);
    }

    public async Task<CancelBookingResponse> CancelBookingAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db
                .Bookings.Include(b => b.Showtime)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Booking);
        if (booking.UserId != userId)
            throw new ForbiddenAppException(Messages.Forbidden_NotBookingOwner);

        if (booking.Status == BookingStatus.Pending)
        {
            await releaser.ReleaseAndCancelBookingAsync(booking, cancellationToken);
            logger.LogInformation(
                "BookingCancelled(Pending) BookingId {BookingId} UserId {UserId}",
                bookingId,
                userId
            );
            return new CancelBookingResponse(
                bookingId,
                BookingStatus.Cancelled.ToDtoStatus(),
                RefundAmount: null,
                RefundStatus: null
            );
        }

        if (booking.Status != BookingStatus.Confirmed)
            throw new BusinessRuleException(
                Messages.Rule_BookingNotCancellable
            );

        var hasCheckedInTicket = await db.BookingTickets.AnyAsync(
            t => t.BookingId == bookingId && t.IsCheckedIn,
            cancellationToken
        );
        if (hasCheckedInTicket)
            throw new BusinessRuleException(
                Messages.Rule_BookingHasCheckedInTicket
            );

        var payment = booking
            .Payments.Where(p => p.Status == PaymentStatus.Success)
            .OrderByDescending(p => p.PaymentId)
            .FirstOrDefault();
        if (payment is null || payment.PaymentMethod == PaymentMethod.Cash)
            throw new BusinessRuleException(
                Messages.Rule_BookingCancelAtCounter
            );

        var settings = await db.GetCinemaSettingsAsync(cancellationToken);

        var eligibility = CalculateRefundEligibility(
            booking.Showtime.StartTime,
            payment.Amount,
            settings
        );
        if (!eligibility.Eligible)
            throw new BusinessRuleException(eligibility.IneligibleReason!);

        var gateway =
            paymentGateways.FindByMethod(payment.PaymentMethod.ToString())
            ?? throw new BusinessRuleException(
                string.Format(Messages.Rule_RefundGatewayNotConfigured, payment.PaymentMethod)
            );

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

        var claimed = await db
            .Bookings.Where(b => b.BookingId == bookingId && b.Status == BookingStatus.Confirmed)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(b => b.Status, BookingStatus.Cancelled),
                cancellationToken
            );
        if (claimed == 0)
            throw new ConflictAppException(Messages.Conflict_BookingBeingProcessedOrCancelled);

        var refundResult = await gateway.RefundAsync(
            new RefundRequest(
                payment.TransactionId!,
                eligibility.RefundAmount,
                string.Format(Messages.Refund_CancelBookingDescription, booking.BookingCode),
                payment.PaidAt ?? payment.CreatedAt,
                GatewayTransactionId: payment.GatewayTransactionId
            ),
            cancellationToken
        );

        if (!refundResult.Success)
        {
            logger.LogWarning(
                "RefundFailed BookingId {BookingId} PaymentId {PaymentId} Message {Message}",
                bookingId,
                payment.PaymentId,
                refundResult.Message
            );
            throw new BusinessRuleException(
                string.Format(Messages.Rule_RefundFailed, refundResult.Message)
            );
        }

        var releasedSeatIds = await ReleaseConfirmedBookingSeatsAsync(booking, cancellationToken);
        booking.Status = BookingStatus.Cancelled;
        payment.Status = PaymentStatus.Refunded;
        payment.RefundedAmount = eligibility.RefundAmount;
        payment.RefundedAt = DateTime.UtcNow;
        payment.RefundTransactionId = refundResult.RefundTransactionId;

        if (booking.UserId is not null)
        {
            var amountToReverse =
                payment.Amount > 0m
                    ? Math.Round(
                        booking.FinalAmount * eligibility.RefundAmount / payment.Amount,
                        0,
                        MidpointRounding.AwayFromZero
                    )
                    : 0m;
            await fulfillmentService.ReverseLoyaltyAsync(
                booking.UserId.Value,
                booking.BookingId,
                amountToReverse,
                settings,
                cancellationToken
            );
        }

        try
        {
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(
                ex,
                "RefundCommitFailed BookingId {BookingId} PaymentId {PaymentId} RefundTransactionId {RefundTransactionId} — đã hoàn tiền ở cổng nhưng DB chưa ghi nhận.",
                bookingId,
                payment.PaymentId,
                refundResult.RefundTransactionId
            );
            throw;
        }

        await realtimeNotifier.NotifySeatReleasedAsync(
            booking.ShowtimeId,
            releasedSeatIds,
            cancellationToken
        );
        logger.LogInformation(
            "BookingCancelled(Confirmed+Refunded) BookingId {BookingId} UserId {UserId} RefundAmount {Amount}",
            bookingId,
            userId,
            eligibility.RefundAmount
        );

        var userEmail = await db
            .Users.Where(u => u.UserId == userId)
            .Select(u => u.Email)
            .FirstAsync(cancellationToken);
        notificationService.SendBookingCancelledRefunded(
            userEmail,
            booking.CustomerName,
            booking.BookingCode,
            eligibility.RefundAmount
        );

        return new CancelBookingResponse(
            bookingId,
            BookingStatus.Cancelled.ToDtoStatus(),
            eligibility.RefundAmount,
            PaymentStatus.Refunded.ToDtoStatus()
        );
    }

    private async Task<List<int>> ReleaseConfirmedBookingSeatsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        var seatIds = await db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .Select(t => t.ShowtimeSeatId)
            .ToListAsync(cancellationToken);

        var seats = await db
            .ShowtimeSeats.Where(ss =>
                seatIds.Contains(ss.ShowtimeSeatId) && ss.Status == ShowtimeSeatStatus.Booked
            )
            .ToListAsync(cancellationToken);

        foreach (var seat in seats)
        {
            seat.Status = ShowtimeSeatStatus.Available;
            seat.HeldByUserId = null;
            seat.HeldByBookingId = null;
            seat.HeldUntil = null;
        }

        return seats.Select(s => s.ShowtimeSeatId).ToList();
    }
}
