namespace Cinema.Application.DTOs.Rooms;

public sealed record SeatLayoutImportRow(
    string? SeatRow,
    string? SeatNumber,
    string? SeatType,
    string? IsActive,
    string? IsAisle,
    string? CoupleGroup,
    int RoomId = 0,
    SeatLayoutItem? Item = null
);
