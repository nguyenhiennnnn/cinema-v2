using Cinema.Application.DTOs.Bookings;

namespace Cinema.Application.Interfaces;

public interface IBookingCancellationService
{
    Task<CancelBookingResponse> CancelBookingAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    );
}
