using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/showtimes")]
[Authorize]
public class AdminShowtimesController(IShowtimeService showtimeService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetShowtimes(
        [FromQuery] AdminShowtimeListFilter filter,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    )
    {
        return Ok(
            await showtimeService.GetShowtimesAdminAsync(filter, page, pageSize, cancellationToken)
        );
    }

    [HttpGet("calendar")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> GetCalendar(
        [FromQuery] AdminShowtimeCalendarFilter filter,
        CancellationToken cancellationToken
    ) => Ok(await showtimeService.GetShowtimesCalendarAsync(filter, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertShowtimeRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await showtimeService.CreateShowtimeAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertShowtimeRequest request,
        CancellationToken cancellationToken
    ) => Ok(await showtimeService.UpdateShowtimeAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        Ok(await showtimeService.DeleteShowtimeAsync(id, cancellationToken));

    [HttpPost("bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkCreate(
        [FromBody] BulkCreateShowtimesRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await showtimeService.BulkCreateShowtimesAsync(request, cancellationToken)
        );

    [HttpPost("auto-schedule/preview")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> PreviewAutoSchedule(
        [FromBody] AutoScheduleRequest request,
        CancellationToken cancellationToken
    ) => Ok(await showtimeService.PreviewAutoScheduleAsync(request, cancellationToken));

    [HttpPost("auto-schedule/apply")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> ApplyAutoSchedule(
        [FromBody] ApplyAutoScheduleRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await showtimeService.ApplyAutoScheduleAsync(request, cancellationToken)
        );

    [HttpDelete("bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkDelete(
        [FromBody] BulkDeleteShowtimesRequest request,
        CancellationToken cancellationToken
    ) => Ok(await showtimeService.BulkDeleteShowtimesAsync(request, cancellationToken));
}
