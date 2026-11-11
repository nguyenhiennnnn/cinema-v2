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

public interface IBookingQueryService
{
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
}
