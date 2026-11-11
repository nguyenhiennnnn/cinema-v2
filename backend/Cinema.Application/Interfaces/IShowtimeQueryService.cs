using Cinema.Application.DTOs.Showtimes;

namespace Cinema.Application.Interfaces;

public record AdminShowtimeCalendarFilter(
    DateOnly From,
    DateOnly To,
    int? RoomId,
    int? MovieId,
    string? Status
);

public interface IShowtimeQueryService
{
    Task<ShowtimeSeatsResponseDto> GetShowtimeSeatsAsync(
        int showtimeId,
        CancellationToken cancellationToken
    );
    Task<List<SeatSuggestionDto>> GetSeatSuggestionsAsync(
        int showtimeId,
        int count,
        CancellationToken cancellationToken
    );
    Task<WholeCinemaScheduleDto> GetWholeCinemaScheduleAsync(
        DateOnly date,
        string? screenType,
        CancellationToken cancellationToken
    );
    Task<ShowtimeCalendarResponseDto> GetShowtimesCalendarAsync(
        AdminShowtimeCalendarFilter filter,
        CancellationToken cancellationToken
    );
}
