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
        var seats = await db
            .ShowtimeSeats.HeldFor(booking, holderUserId)
            .ToListAsync(cancellationToken);

        var seatIds = seats.Select(s => s.ShowtimeSeatId).ToList();
        foreach (var seat in seats)
        {
            seat.Status = ShowtimeSeatStatus.Available;
            seat.HeldByUserId = null;
            seat.HeldByBookingId = null;
            seat.HeldUntil = null;
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
