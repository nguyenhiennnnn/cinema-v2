namespace Cinema.Application.DTOs.Showtimes;

public record ShowtimeSeatItemDto(
    int ShowtimeSeatId,
    string SeatRow,
    int SeatNumber,
    int? SeatTypeId,
    string? SeatType,
    Guid? CoupleId,
    string Status,
    decimal Price
);

public record AislePositionDto(string SeatRow, int SeatNumber);

public record ShowtimeSeatsResponseDto(
    int ShowtimeId,
    string RoomName,
    string MovieTitle,
    DateTime StartTime,
    int Rows,
    int Cols,
    List<AislePositionDto> Aisles,
    List<ShowtimeSeatItemDto> Seats
);

public record SeatSuggestionDto(
    string SeatRow,
    List<int> SeatNumbers,
    List<int> ShowtimeSeatIds,
    decimal TotalPrice,
    double Score
);

public record AdminShowtimeDto(
    int ShowtimeId,
    int MovieId,
    string MovieTitle,
    int RoomId,
    string RoomName,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice,
    string Status
);

public record CreateShowtimeResponse(
    int ShowtimeId,
    int MovieId,
    string MovieTitle,
    int RoomId,
    string RoomName,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice,
    string Status,
    int SeatsGenerated
);

public record UpsertShowtimeRequest(int MovieId, int RoomId, DateTime StartTime, decimal BasePrice);

public record DeleteShowtimeResponse(int ShowtimeId, string Status, int ManualRefundRequiredCount);

public record WholeCinemaShowtimeSlotDto(
    int ShowtimeId,
    int RoomId,
    string RoomName,
    string? ScreenType,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice,
    int AvailableSeats,
    int TotalSeats,
    string Status
);

public record WholeCinemaMovieDto(
    int MovieId,
    string MovieTitle,
    string? MoviePosterUrl,
    List<WholeCinemaShowtimeSlotDto> Showtimes
);

public record WholeCinemaScheduleDto(string Date, List<WholeCinemaMovieDto> Movies);

public record BulkDateRange(DateOnly From, DateOnly To);

public record BulkCreateShowtimesRequest(
    int MovieId,
    int RoomId,
    BulkDateRange DateRange,
    List<string> DaysOfWeek,
    List<string> Times,
    decimal BasePrice
);

public record BulkCreatedShowtimeItem(int ShowtimeId, DateTime StartTime, int SeatsGenerated);

public record BulkSkippedShowtimeItem(string Date, string Time, string Reason);

public record BulkCreateShowtimesResponse(
    List<BulkCreatedShowtimeItem> Created,
    List<BulkSkippedShowtimeItem> Skipped,
    int TotalCreated,
    int TotalSkipped
);

public record BulkDeleteShowtimesRequest(List<int> ShowtimeIds);

public record BulkDeleteShowtimesResultItem(int ShowtimeId, bool Success, string? Reason);

public record BulkDeleteShowtimesResponse(
    List<BulkDeleteShowtimesResultItem> Results,
    int TotalSuccess,
    int TotalFailed
);

// ── Admin showtime calendar (theo khoảng ngày) ─────────────────────────────────

public record ShowtimeCalendarItemDto(
    int ShowtimeId,
    int MovieId,
    string MovieTitle,
    string? MoviePosterUrl,
    int RoomId,
    string RoomName,
    string? ScreenType,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice,
    int AvailableSeats,
    int TotalSeats,
    string Status
);

public record ShowtimeCalendarDayDto(string Date, List<ShowtimeCalendarItemDto> Showtimes);

public record ShowtimeCalendarResponseDto(
    string From,
    string To,
    List<ShowtimeCalendarDayDto> Days
);
