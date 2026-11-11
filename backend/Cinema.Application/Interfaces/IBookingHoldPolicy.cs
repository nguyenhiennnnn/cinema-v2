using Cinema.Domain.Entities;
using Cinema.Domain.Enums;

namespace Cinema.Application.Interfaces;

public record BookingHoldPolicyContext(int ShowtimeId, int HolderUserId, int? CreatedByEmployeeId);

public interface IBookingHoldPolicy
{
    BookingChannel Channel { get; }
    void EnsureSalesOpen(Showtime showtime, int stopSellingMinutesBefore);
    Task EnsureNoConflictingPendingAsync(
        BookingHoldPolicyContext context,
        CancellationToken cancellationToken
    );
}
