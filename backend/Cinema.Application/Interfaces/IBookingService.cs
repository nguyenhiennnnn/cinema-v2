using Cinema.Application.DTOs.Bookings;
using Cinema.Application.DTOs.Common;

namespace Cinema.Application.Interfaces;

public record AdminBookingListFilter(
    string? Status,
    string? Channel,
    DateOnly? FromDate,
    DateOnly? ToDate,
    string? Search
);

public interface IBookingService
{
    Task<HoldBookingResponse> HoldSeatsAsync(
        int userId,
        HoldSeatsRequest request,
        CancellationToken cancellationToken
    );
    Task<CursorPageResult<BookingListItemDto>> GetMyBookingsAsync(
        int userId,
        string? status,
        string? cursor,
        int limit,
        CancellationToken cancellationToken
    );
    Task<BookingDetailDto> GetBookingDetailAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    );
    Task<CheckoutResponse> CheckoutAsync(
        int userId,
        int bookingId,
        CheckoutRequest request,
        CancellationToken cancellationToken
    );
    Task<CancelBookingResponse> CancelBookingAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    );

    Task<CounterHoldResponse> CounterHoldSeatsAsync(
        int employeeId,
        int employeeUserId,
        CounterHoldRequest request,
        CancellationToken cancellationToken
    );
    Task<OffsetPageResult<AdminBookingListItemDto>> GetBookingsAdminAsync(
        AdminBookingListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<AdminBookingDetailDto> GetBookingDetailAdminAsync(
        int bookingId,
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

    Task ConfirmBookingAndGenerateTicketsAsync(int bookingId, CancellationToken cancellationToken);
    Task ProcessPaymentFailureAsync(int bookingId, CancellationToken cancellationToken);
    Task ReverseLoyaltyAsync(
        int userId,
        int bookingId,
        decimal finalAmount,
        CancellationToken cancellationToken
    );
}
