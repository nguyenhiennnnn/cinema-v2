using Cinema.Api.Filters;
using Cinema.Application.DTOs.Rooms;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/rooms")]
[Authorize]
public class AdminRoomsController(IRoomService roomService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetRooms(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await roomService.GetRoomsAsync(page, pageSize, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_RoomCreated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await roomService.CreateRoomAsync(request, cancellationToken)
        );

    [HttpGet("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        Ok(await roomService.GetRoomByIdAsync(id, cancellationToken));

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_RoomUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateRoomRequest request,
        CancellationToken cancellationToken
    ) => Ok(await roomService.UpdateRoomAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_RoomDeleted))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        Ok(await roomService.DeleteRoomAsync(id, cancellationToken));

    [HttpGet("{id:int}/seats")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetSeats(int id, CancellationToken cancellationToken) =>
        Ok(await roomService.GetRoomSeatsAsync(id, cancellationToken));

    [HttpPut("{id:int}/seats")]
    [SuccessMessage(nameof(Messages.Success_RoomSeatsSaved))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> UpdateSeatLayout(
        int id,
        [FromBody] UpdateSeatLayoutRequest request,
        CancellationToken cancellationToken
    ) => Ok(await roomService.UpdateSeatLayoutAsync(id, request, cancellationToken));
}
