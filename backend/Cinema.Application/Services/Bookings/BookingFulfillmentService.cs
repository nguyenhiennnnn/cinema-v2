using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Bookings;

public class BookingFulfillmentService(
    IAppDbContext db,
    IOptions<BookingOptions> bookingOptions,
    IRealtimeNotifier realtimeNotifier,
    INotificationService notificationService,
    BookingSeatReleaser releaser,
    ILogger<BookingFulfillmentService> logger
) : IBookingFulfillmentService
{
    private readonly BookingOptions _options = bookingOptions.Value;

    public async Task ConfirmBookingAndGenerateTicketsAsync(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Booking);

        if (booking.Status == BookingStatus.Confirmed)
        {
            logger.LogInformation("PaymentAlreadyProcessed BookingId {BookingId}", bookingId);
            return;
        }

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Expired)
            throw new SeatsNoLongerHeldException(bookingId);

        var holderUserId = await releaser.ResolveHolderUserIdAsync(booking, cancellationToken);
        var heldSeats = await db
            .ShowtimeSeats.Include(ss => ss.Seat)
            .HeldFor(booking, holderUserId)
            .ToListAsync(cancellationToken);

        if (heldSeats.Count == 0)
        {
            booking.Status = BookingStatus.Cancelled;
            await db.SaveChangesAsync(cancellationToken);
            logger.LogWarning(
                "ConfirmBookingFailed_SeatsExpired BookingId {BookingId} — ghế đã hết hạn giữ trước khi confirm.",
                bookingId
            );
            throw new SeatsNoLongerHeldException(bookingId);
        }

        var ownsConfirmTransaction = db.Database.CurrentTransaction is null;
        var transaction = ownsConfirmTransaction
            ? await db.Database.BeginTransactionAsync(cancellationToken)
            : db.Database.CurrentTransaction!;

        var seatIds = new List<int>();
        try
        {
            booking.Status = BookingStatus.Confirmed;

            var heldSeatIds = heldSeats.Select(s => s.ShowtimeSeatId).ToList();
            await db
                .BookingTickets.Where(t =>
                    heldSeatIds.Contains(t.ShowtimeSeatId)
                    && t.Booking.Status != BookingStatus.Confirmed
                )
                .ExecuteDeleteAsync(cancellationToken);

            foreach (var seat in heldSeats)
            {
                seat.Status = ShowtimeSeatStatus.Booked;
                seat.HeldByUserId = null;
                seat.HeldByBookingId = null;
                seat.HeldUntil = null;
                seatIds.Add(seat.ShowtimeSeatId);

                db.BookingTickets.Add(
                    new BookingTicket
                    {
                        BookingId = booking.BookingId,
                        ShowtimeSeatId = seat.ShowtimeSeatId,
                        Price = seat.Price,
                        QrCode = CodeGenerator.GenerateTicketQrPayload(
                            booking.BookingCode,
                            seat.Seat.SeatRow,
                            seat.Seat.SeatNumber
                        ),
                        IsCheckedIn = false,
                    }
                );
            }

            if (booking.UserId is not null)
                await ApplyLoyaltyAsync(
                    booking.UserId.Value,
                    booking.BookingId,
                    booking.FinalAmount,
                    cancellationToken
                );

            await db.SaveChangesAsync(cancellationToken);
            if (ownsConfirmTransaction)
                await transaction.CommitAsync(cancellationToken);

            await realtimeNotifier.NotifySeatBookedAsync(
                booking.ShowtimeId,
                seatIds,
                cancellationToken
            );
            logger.LogInformation(
                "BookingCreated->Confirmed BookingId {BookingId} SeatsBooked {Count}",
                bookingId,
                seatIds.Count
            );
        }
        finally
        {
            if (ownsConfirmTransaction)
                await transaction.DisposeAsync();
        }

        if (booking.UserId is not null)
        {
            var email = await db
                .Users.Where(u => u.UserId == booking.UserId)
                .Select(u => u.Email)
                .FirstAsync(cancellationToken);
            var movieTitle = await db
                .Showtimes.Where(s => s.ShowtimeId == booking.ShowtimeId)
                .Select(s => s.Movie.Title)
                .FirstOrDefaultAsync(cancellationToken);
            notificationService.SendBookingConfirmed(
                email,
                booking.CustomerName,
                booking.BookingCode,
                movieTitle ?? string.Empty,
                seatIds.Count
            );
            logger.LogInformation(
                "EmailQueued BookingId {BookingId} Type TicketConfirmation",
                bookingId
            );
        }
    }

    public async Task ProcessPaymentFailureAsync(int bookingId, CancellationToken cancellationToken)
    {
        var booking =
            await db.Bookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Booking);

        if (booking.Status != BookingStatus.Pending)
        {
            logger.LogInformation(
                "ProcessPaymentFailure bỏ qua — BookingId {BookingId} không ở trạng thái Pending.",
                bookingId
            );
            return;
        }

        if (booking.ExpiresAt > DateTime.UtcNow)
        {
            logger.LogInformation(
                "ProcessPaymentFailure giữ ghế — BookingId {BookingId} còn hạn, cho phép thanh toán lại.",
                bookingId
            );
            return;
        }

        await releaser.ReleaseAndCancelBookingAsync(booking, cancellationToken);
        logger.LogInformation(
            "PaymentFailed BookingId {BookingId} — ghế đã được release.",
            bookingId
        );
    }

    public async Task ReverseLoyaltyAsync(
        int userId,
        int bookingId,
        decimal finalAmount,
        CancellationToken cancellationToken
    )
    {
        var settings = await db.GetCinemaSettingsAsync(cancellationToken);
        await ReverseLoyaltyAsync(userId, bookingId, finalAmount, settings, cancellationToken);
    }

    public async Task ReverseLoyaltyAsync(
        int userId,
        int bookingId,
        decimal finalAmount,
        CinemaSettings settings,
        CancellationToken cancellationToken
    )
    {
        var profile = await db.CustomerProfiles.FirstOrDefaultAsync(
            p => p.UserId == userId,
            cancellationToken
        );
        if (profile is null)
            return;

        // Chỉ trừ vào đúng dòng điểm của booking đó: nếu dòng đã trượt khỏi cửa sổ 12 tháng
        // thì hoàn tiền không làm giảm điểm của cửa sổ hiện tại.
        var entry = await db.LoyaltyPointEntries.FirstOrDefaultAsync(
            e => e.BookingId == bookingId,
            cancellationToken
        );
        if (entry is not null)
        {
            var revoke = LoyaltyWindow.PointsFor(finalAmount, settings.PointsPerAmount);
            entry.RevokedPoints = Math.Min(entry.Points, entry.RevokedPoints + revoke);
        }

        profile.TotalSpent = Math.Max(0, profile.TotalSpent - finalAmount);
        await LoyaltyWindow.ResyncProfileAsync(
            db,
            profile,
            pendingNewPoints: 0,
            DateTime.UtcNow,
            cancellationToken
        );
    }

    private async Task ApplyLoyaltyAsync(
        int userId,
        int bookingId,
        decimal finalAmount,
        CancellationToken cancellationToken
    )
    {
        var profile = await db.CustomerProfiles.FirstOrDefaultAsync(
            p => p.UserId == userId,
            cancellationToken
        );
        if (profile is null)
            return;

        var settings = await db.GetCinemaSettingsAsync(cancellationToken);
        var now = DateTime.UtcNow;
        var earned = LoyaltyWindow.PointsFor(finalAmount, settings.PointsPerAmount);

        if (earned > 0)
            db.LoyaltyPointEntries.Add(
                new LoyaltyPointEntry
                {
                    UserId = userId,
                    BookingId = bookingId,
                    Points = earned,
                    EarnedAt = now,
                }
            );

        var oldTierId = profile.TierId;
        profile.TotalSpent += finalAmount;
        // Entry mới chưa Save nên cộng thêm `earned` vào tổng các dòng đã lưu.
        await LoyaltyWindow.ResyncProfileAsync(db, profile, earned, now, cancellationToken);

        if (profile.TierId != oldTierId)
            logger.LogInformation(
                "TierChanged UserId {UserId} FromTierId {From} ToTierId {To} Points {Points}",
                userId,
                oldTierId,
                profile.TierId,
                profile.Points
            );
    }

    public async Task<string> GenerateUniqueBookingCodeAsync(CancellationToken cancellationToken)
    {
        var maxAttempts = Math.Max(1, _options.BookingCodeMaxAttempts);
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var code = CodeGenerator.GenerateBookingCode();
            if (!await db.Bookings.AnyAsync(b => b.BookingCode == code, cancellationToken))
                return code;
        }
        throw new BusinessRuleException(Messages.Rule_BookingCodeGenerationFailed);
    }
}
