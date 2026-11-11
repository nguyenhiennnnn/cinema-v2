using Cinema.Application.DTOs.Bookings;

namespace Cinema.Application.Interfaces;

public interface ICounterBookingService
{
    Task<CounterHoldResponse> CounterHoldSeatsAsync(
        int employeeId,
        int employeeUserId,
        CounterHoldRequest request,
        CancellationToken cancellationToken
    );
    Task<CancelBookingResponse> CounterCancelAsync(
        int bookingId,
        int employeeId,
        CancellationToken cancellationToken
    );
    Task<CounterCheckoutResponse> CounterCheckoutAsync(
        int bookingId,
        CounterCheckoutRequest request,
        CancellationToken cancellationToken
    );
    Task<CheckinResponse> CheckinAsync(CheckinRequest request, CancellationToken cancellationToken);
}
