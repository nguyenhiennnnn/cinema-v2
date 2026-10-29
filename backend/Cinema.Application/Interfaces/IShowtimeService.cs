using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Showtimes;

namespace Cinema.Application.Interfaces;

public record AdminShowtimeListFilter(int? RoomId, int? MovieId, DateOnly? Date, string? Status);

public record AdminShowtimeCalendarFilter(
    DateOnly From,
    DateOnly To,
    int? RoomId,
    int? MovieId,
    string? Status
);

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
    Task<DeleteShowtimeResponse> DeleteShowtimeAsync(
        int showtimeId,
        CancellationToken cancellationToken
    );
    Task<BulkCreateShowtimesResponse> BulkCreateShowtimesAsync(
        BulkCreateShowtimesRequest request,
        CancellationToken cancellationToken
    );
    Task<BulkDeleteShowtimesResponse> BulkDeleteShowtimesAsync(
        BulkDeleteShowtimesRequest request,
        CancellationToken cancellationToken
    );

    Task<int> CompleteEndedShowtimesAsync(CancellationToken cancellationToken);

    Task<int> EndMoviesWithoutUpcomingShowtimesAsync(CancellationToken cancellationToken);

    Task<AutoScheduleResponseDto> PreviewAutoScheduleAsync(
        AutoScheduleRequest request,
        CancellationToken cancellationToken
    );

    Task<BulkCreateShowtimesResponse> ApplyAutoScheduleAsync(
        ApplyAutoScheduleRequest request,
        CancellationToken cancellationToken
    );

    Task<BulkCreateShowtimesResponse> CloneShowtimesAsync(
        CloneShowtimesRequest request,
        CancellationToken cancellationToken
    );

    Task<WholeCinemaScheduleDto> GetWholeCinemaScheduleAsync(
        DateOnly date,
        CancellationToken cancellationToken
    );

    Task<ShowtimeCalendarResponseDto> GetShowtimesCalendarAsync(
        AdminShowtimeCalendarFilter filter,
        CancellationToken cancellationToken
    );

    Task<ShowtimeSeatsResponseDto> GetShowtimeSeatsAsync(
        int showtimeId,
        CancellationToken cancellationToken
    );

    Task<List<SeatSuggestionDto>> GetSeatSuggestionsAsync(
        int showtimeId,
        int count,
        CancellationToken cancellationToken
    );
}
