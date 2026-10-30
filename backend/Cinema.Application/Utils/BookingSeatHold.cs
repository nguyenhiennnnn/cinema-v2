using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Utils;

public static class BookingSeatHold
{
    public static async Task<int> ResolveHolderUserIdAsync(
        IAppDbContext db,
        Booking booking,
        CancellationToken cancellationToken
    )
    {
        if (booking.Channel == BookingChannel.Counter && booking.CreatedByEmployeeId is not null)
            return await db
                .Employees.Where(e => e.EmployeeId == booking.CreatedByEmployeeId)
                .Select(e => e.UserId)
                .FirstAsync(cancellationToken);

        if (booking.UserId is not null)
            return booking.UserId.Value;

        if (booking.CreatedByEmployeeId is not null)
            return await db
                .Employees.Where(e => e.EmployeeId == booking.CreatedByEmployeeId)
                .Select(e => e.UserId)
                .FirstAsync(cancellationToken);

        throw new BusinessRuleException("Booking thiếu cả UserId lẫn CreatedByEmployeeId.");
    }

    public static IQueryable<ShowtimeSeat> HeldFor(
        this IQueryable<ShowtimeSeat> seats,
        Booking booking,
        int holderUserId
    )
    {
        var bookingId = booking.BookingId;
        var showtimeId = booking.ShowtimeId;
        return seats.Where(ss =>
            ss.ShowtimeId == showtimeId
            && ss.Status == ShowtimeSeatStatus.Held
            && (
                ss.HeldByBookingId == bookingId
                || (ss.HeldByBookingId == null && ss.HeldByUserId == holderUserId)
            )
        );
    }
}
