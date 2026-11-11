using Cinema.Application.DTOs.Bookings;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;

namespace Cinema.Application.Interfaces;

public interface IBookingHoldService
{
    Task<HoldBookingResponse> HoldSeatsAsync(
        int userId,
        HoldSeatsRequest request,
        CancellationToken cancellationToken
    );
    Task<Booking> CreateHoldBookingAsync(
        int showtimeId,
        List<int> showtimeSeatIds,
        List<ComboSelectionRequest>? comboSelections,
        int holderUserId,
        BookingChannel channel,
        int? bookingUserId,
        int? createdByEmployeeId,
        string customerName,
        string? customerPhone,
        CancellationToken cancellationToken
    );
    Task EnsureWithinBookingLimitsAsync(
        int seatCount,
        List<ComboSelectionRequest>? combos,
        CancellationToken cancellationToken
    );
}
