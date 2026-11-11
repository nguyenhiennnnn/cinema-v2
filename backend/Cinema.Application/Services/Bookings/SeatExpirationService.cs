using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Bookings;

public class SeatExpirationService(
    IAppDbContext db,
    IRealtimeNotifier realtimeNotifier,
    BookingSeatReleaser seatReleaser,
    ILogger<SeatExpirationService> logger
) : ISeatExpirationService
{
    public async Task<int> ReleaseStaleHeldSeatsAsync(
        List<int>? showtimeIds,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.UtcNow;

        var query = db.ShowtimeSeats.Where(ss =>
            ss.Status == ShowtimeSeatStatus.Held && ss.HeldUntil != null && ss.HeldUntil < now
        );

        if (showtimeIds is not null)
        {
            if (showtimeIds.Count == 0)
                return 0;

            query = query.Where(ss => showtimeIds.Contains(ss.ShowtimeId));
        }

        return await query.ExecuteUpdateAsync(
            setters =>
                setters
                    .SetProperty(x => x.Status, ShowtimeSeatStatus.Available)
                    .SetProperty(x => x.HeldByUserId, (int?)null)
                    .SetProperty(x => x.HeldByBookingId, (int?)null)
                    .SetProperty(x => x.HeldUntil, (DateTime?)null),
            cancellationToken
        );
    }

    public async Task ExpireDueBookingsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var expirationBookings = await db
            .Bookings.Where(b =>
                b.Status == BookingStatus.Pending && b.ExpiresAt != null && b.ExpiresAt < now
            )
            .ToListAsync(cancellationToken);

        foreach (var booking in expirationBookings)
        {
            var seatIds = await seatReleaser.ReleaseBookingSeatsAsync(booking, cancellationToken);

            booking.Status = BookingStatus.Expired;

            await db
                .Payments.Where(p =>
                    p.BookingId == booking.BookingId && p.Status == PaymentStatus.Pending
                )
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(p => p.Status, PaymentStatus.Failed),
                    cancellationToken
                );

            await db.SaveChangesAsync(cancellationToken);

            if (seatIds.Count > 0)
                await realtimeNotifier.NotifyHoldExpiredAsync(
                    booking.ShowtimeId,
                    booking.BookingId,
                    seatIds,
                    cancellationToken
                );

            logger.LogInformation(
                "BookingExpired BookingId {BookingId} SeatsReleased {Count}",
                booking.BookingId,
                seatIds.Count
            );
        }

        var staleReleased = await ReleaseStaleHeldSeatsAsync(null, cancellationToken);
        if (staleReleased > 0)
            logger.LogInformation(
                "SeatsReleased (stale, không khớp booking Pending nào) Count {Count}",
                staleReleased
            );
    }
}
