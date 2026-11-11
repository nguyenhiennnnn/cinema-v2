using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Bookings;

public class BookingSeatReleaser(IAppDbContext db, IRealtimeNotifier realtimeNotifier)
{
    public Task<int> ResolveHolderUserIdAsync(
        Booking booking,
        CancellationToken cancellationToken
    ) => BookingSeatHold.ResolveHolderUserIdAsync(db, booking, cancellationToken);

    public async Task<List<int>> ReleaseBookingSeatsAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        var holderUserId = await ResolveHolderUserIdAsync(booking, cancellationToken);
        var seatIds = await db
            .ShowtimeSeats.HeldFor(booking, holderUserId)
            .Select(s => s.ShowtimeSeatId)
            .ToListAsync(cancellationToken);

        if (seatIds.Count > 0)
        {
            await db
                .ShowtimeSeats.Where(ss => seatIds.Contains(ss.ShowtimeSeatId))
                .ExecuteUpdateAsync(
                    setters =>
                        setters
                            .SetProperty(x => x.Status, ShowtimeSeatStatus.Available)
                            .SetProperty(x => x.HeldByUserId, (int?)null)
                            .SetProperty(x => x.HeldByBookingId, (int?)null)
                            .SetProperty(x => x.HeldUntil, (DateTime?)null),
                    cancellationToken
                );
        }

        return seatIds;
    }

    public async Task ReleaseAndCancelBookingAsync(
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        var releasedSeatIds = await ReleaseBookingSeatsAsync(booking, cancellationToken);
        booking.Status = BookingStatus.Cancelled;

        await db
            .Payments.Where(p =>
                p.BookingId == booking.BookingId && p.Status == PaymentStatus.Pending
            )
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(p => p.Status, PaymentStatus.Failed),
                cancellationToken
            );

        await db.SaveChangesAsync(cancellationToken);

        await realtimeNotifier.NotifySeatReleasedAsync(
            booking.ShowtimeId,
            releasedSeatIds,
            cancellationToken
        );
    }
}
