using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Bookings;

public class CounterHoldPolicy(IAppDbContext db) : IBookingHoldPolicy
{
    private const int MaxCounterPendingBookingsPerEmployee = 5;

    public BookingChannel Channel => BookingChannel.Counter;

    public void EnsureSalesOpen(Showtime showtime, int stopSellingMinutesBefore) { }

    public async Task EnsureNoConflictingPendingAsync(
        BookingHoldPolicyContext context,
        CancellationToken cancellationToken
    )
    {
        var counterPendingCount = await db.Bookings.CountAsync(
            b =>
                b.Channel == BookingChannel.Counter
                && b.Status == BookingStatus.Pending
                && b.ExpiresAt > DateTime.UtcNow
                && b.CreatedByEmployeeId == context.CreatedByEmployeeId,
            cancellationToken
        );
        if (counterPendingCount >= MaxCounterPendingBookingsPerEmployee)
            throw new ConflictAppException(Messages.Rule_CounterPendingLimit);
    }
}
