using Cinema.Api.Filters;
using Cinema.Application.DTOs.Import;
using Cinema.Application.DTOs.Rooms;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/rooms")]
[Authorize]
public class AdminRoomsController(
    IRoomService roomService,
    ISeatLayoutImportService seatLayoutImportService
) : AppControllerBase
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

    [HttpGet("{id:int}/seat-layout/export")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> ExportSeatLayout(
        int id,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    )
    {
        var file = await seatLayoutImportService.ExportAsync(
            id,
            ExportFormats.Parse(format),
            cancellationToken
        );
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpGet("{id:int}/seat-layout/import/template")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> DownloadSeatLayoutImportTemplate(
        int id,
        CancellationToken cancellationToken
    )
    {
        var file = await seatLayoutImportService.GetTemplateAsync(id, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost("{id:int}/seat-layout/import/preview")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(ImportLimits.MaxRequestBytes)]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> PreviewSeatLayoutImport(
        int id,
        IFormFile? file = null,
        CancellationToken cancellationToken = default
    )
    {
        if (file is null || file.Length == 0)
            throw new ValidationAppException(
                Messages.Invalid_UploadFileMissing,
                new Dictionary<string, string[]> { ["file"] = [Messages.Invalid_FileEmpty] }
            );

        await using var stream = file.OpenReadStream();
        return Ok(
            await seatLayoutImportService.PreviewAsync(
                id,
                CurrentUserId,
                file.FileName,
                stream,
                cancellationToken
            )
        );
    }

    [HttpPost("{id:int}/seat-layout/import/apply")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> ApplySeatLayoutImport(
        int id,
        [FromBody] ImportApplyRequest request,
        CancellationToken cancellationToken
    ) =>
        Ok(await seatLayoutImportService.ApplyAsync(id, CurrentUserId, request, cancellationToken));

    [HttpGet("{id:int}/seat-layout/import/errors/{previewId}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> DownloadSeatLayoutImportErrors(
        int id,
        string previewId,
        CancellationToken cancellationToken
    )
    {
        var file = await seatLayoutImportService.GetErrorFileAsync(
            id,
            CurrentUserId,
            previewId,
            cancellationToken
        );
        return File(file.Content, file.ContentType, file.FileName);
    }
}
