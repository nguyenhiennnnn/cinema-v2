using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;

namespace Cinema.Application.Services.Bookings;

public class CounterHoldPolicy : IBookingHoldPolicy
{
    public BookingChannel Channel => BookingChannel.Counter;

    public void EnsureSalesOpen(Showtime showtime, int stopSellingMinutesBefore) { }

    public Task EnsureNoConflictingPendingAsync(
        BookingHoldPolicyContext context,
        CancellationToken cancellationToken
    ) => Task.CompletedTask;
}
