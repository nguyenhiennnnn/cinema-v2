using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Showtimes;

namespace Cinema.Application.Interfaces;

public record AdminShowtimeListFilter(int? RoomId, int? MovieId, DateOnly? Date, string? Status);

public interface IShowtimeService
{
    Task<OffsetPageResult<AdminShowtimeDto>> GetShowtimesAdminAsync(
        AdminShowtimeListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<CreateShowtimeResponse> CreateShowtimeAsync(
        UpsertShowtimeRequest request,
        CancellationToken cancellationToken
    );
    Task<AdminShowtimeDto> UpdateShowtimeAsync(
        int showtimeId,
        UpsertShowtimeRequest request,
        CancellationToken cancellationToken
    );
}
