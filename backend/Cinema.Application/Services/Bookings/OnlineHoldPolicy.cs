using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Bookings;

public class OnlineHoldPolicy(IAppDbContext db) : IBookingHoldPolicy
{
    public BookingChannel Channel => BookingChannel.Online;

    public void EnsureSalesOpen(Showtime showtime, int stopSellingMinutesBefore)
    {
        if (
            ShowtimeLifecycle.IsOnlineSalesClosed(
                DateTime.UtcNow,
                showtime.StartTime,
                stopSellingMinutesBefore
            )
        )
            throw new BusinessRuleException(
                string.Format(Messages.Rule_OnlineSalesClosed, stopSellingMinutesBefore)
            );
    }

    public async Task EnsureNoConflictingPendingAsync(
        BookingHoldPolicyContext context,
        CancellationToken cancellationToken
    )
    {
        var hasActivePending = await db.Bookings.AnyAsync(
            b =>
                b.ShowtimeId == context.ShowtimeId
                && b.Status == BookingStatus.Pending
                && b.ExpiresAt > DateTime.UtcNow
                && b.Channel == BookingChannel.Online
                && b.UserId == context.HolderUserId,
            cancellationToken
        );
        if (hasActivePending)
            throw new ConflictAppException(Messages.Conflict_PendingBookingExistsForShowtime);
    }
}
