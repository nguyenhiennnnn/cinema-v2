using Cinema.Application.DTOs.Bookings;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Api.Filters;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/bookings")]
[Authorize]
public class AdminBookingsController(
    IBookingQueryService bookingQueryService,
    IExportJobService exportJobService
) : AppControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<ActionResult<OffsetPageResult<AdminBookingListItemDto>>> GetBookings(
        [FromQuery] AdminBookingListFilter filter,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        var result = await bookingQueryService.GetBookingsAdminAsync(
            filter,
            page,
            pageSize,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("export")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> ExportBookings(
        [FromQuery] AdminBookingListFilter filter,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    )
    {
        var exportFormat = ExportFormats.Parse(format);
        var file = await exportJobService.ExportNowAsync(
            CurrentUserId,
            ImportExportModules.Bookings,
            filter,
            exportFormat,
            cancellationToken
        );
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost("export-jobs")]
    [SuccessMessage(nameof(Messages.Success_ExportJobQueued))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> CreateExportJob(
        [FromQuery] AdminBookingListFilter filter,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status202Accepted,
            await exportJobService.EnqueueAsync(
                CurrentUserId,
                ImportExportModules.Bookings,
                filter,
                ExportFormats.Parse(format),
                cancellationToken
            )
        );

    [HttpGet("{bookingId:int}")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<ActionResult<AdminBookingDetailDto>> GetBookingDetail(
        int bookingId,
        CancellationToken cancellationToken
    )
    {
        var result = await bookingQueryService.GetBookingDetailAdminAsync(
            bookingId,
            cancellationToken
        );
        return Ok(result);
    }
}
