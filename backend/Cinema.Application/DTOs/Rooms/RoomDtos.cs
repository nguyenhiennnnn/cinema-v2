namespace Cinema.Application.DTOs.Rooms;

public record AdminRoomDto(
    int RoomId,
    string RoomName,
    int? ScreenTypeId,
    string? ScreenTypeName,
    int TotalSeats,
    bool IsActive,
    int Rows,
    int Cols
);

public record CreateRoomRequest(string RoomName, int? ScreenTypeId, int Rows, int Cols);

public record UpdateRoomRequest(string RoomName, int? ScreenTypeId, bool IsActive);

public record DeleteRoomResponse(int RoomId, bool Deleted, bool SoftDeleted);

public record SeatDto(
    int SeatId,
    string SeatRow,
    int SeatNumber,
    int? SeatTypeId,
    string? SeatTypeName,
    bool IsActive,
    bool IsAisle,
    Guid? CoupleId
);

public record SeatLayoutItem(
    string SeatRow,
    int SeatNumber,
    int? SeatTypeId,
    bool IsActive,
    bool IsAisle,
    Guid? CoupleId
);

public record UpdateSeatLayoutRequest(int Rows, int Cols, List<SeatLayoutItem> Seats);

public record UpdateSeatLayoutResponse(
    int Created,
    int Updated,
    int Deactivated,
    int Removed,
    int TotalSeats
);
