using Cinema.Application.DTOs.Bookings;

namespace Cinema.Application.Interfaces;

public interface IBookingCheckoutService
{
    Task<CheckoutResponse> CheckoutAsync(
        int userId,
        int bookingId,
        CheckoutRequest request,
        CancellationToken cancellationToken
    );
}
