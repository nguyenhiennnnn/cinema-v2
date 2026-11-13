using Cinema.Application.DTOs.Bookings;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/bookings")]
[Authorize]
public class AdminBookingsController(
    IBookingQueryService bookingQueryService,
    IBookingExportService bookingExportService
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
        var file = await bookingExportService.ExportBookingsAsync(
            filter,
            exportFormat,
            cancellationToken
        );
        return File(file.Content, file.ContentType, file.FileName);
    }

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
