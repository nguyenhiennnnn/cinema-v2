namespace Cinema.Application.DTOs.Showtimes;

public sealed record ShowtimeImportRow(
    string? Movie,
    string? Room,
    string? StartTime,
    string? BasePrice,
    string? AllowAfterHours,
    UpsertShowtimeRequest? Request = null
);
