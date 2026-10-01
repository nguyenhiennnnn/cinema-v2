using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Rooms;

namespace Cinema.Application.Interfaces;

public interface IRoomService
{
    Task<OffsetPageResult<AdminRoomDto>> GetRoomsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task<AdminRoomDto> GetRoomByIdAsync(int roomId, CancellationToken cancellationToken);
    Task<AdminRoomDto> CreateRoomAsync(
        CreateRoomRequest request,
        CancellationToken cancellationToken
    );
    Task<AdminRoomDto> UpdateRoomAsync(
        int roomId,
        UpdateRoomRequest request,
        CancellationToken cancellationToken
    );
    Task<DeleteRoomResponse> DeleteRoomAsync(int roomId, CancellationToken cancellationToken);

    Task<List<SeatDto>> GetRoomSeatsAsync(int roomId, CancellationToken cancellationToken);
    Task<UpdateSeatLayoutResponse> UpdateSeatLayoutAsync(
        int roomId,
        UpdateSeatLayoutRequest request,
        CancellationToken cancellationToken
    );
}
