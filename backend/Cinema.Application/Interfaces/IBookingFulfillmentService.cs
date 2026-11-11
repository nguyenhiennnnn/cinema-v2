using Cinema.Domain.Entities;

namespace Cinema.Application.Interfaces;

public interface IBookingFulfillmentService
{
    Task ConfirmBookingAndGenerateTicketsAsync(int bookingId, CancellationToken cancellationToken);
    Task ProcessPaymentFailureAsync(int bookingId, CancellationToken cancellationToken);
    Task ReverseLoyaltyAsync(
        int userId,
        int bookingId,
        decimal finalAmount,
        CancellationToken cancellationToken
    );
    Task ReverseLoyaltyAsync(
        int userId,
        int bookingId,
        decimal finalAmount,
        CinemaSettings settings,
        CancellationToken cancellationToken
    );
    Task<string> GenerateUniqueBookingCodeAsync(CancellationToken cancellationToken);
}
