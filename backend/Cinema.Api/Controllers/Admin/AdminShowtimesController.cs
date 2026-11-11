using Cinema.Api.Filters;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/showtimes")]
[Authorize]
public class AdminShowtimesController(
    IShowtimeService showtimeService,
    IShowtimeCancellationService showtimeCancellationService,
    IShowtimeBatchService showtimeBatchService,
    IShowtimeAutoScheduleService showtimeAutoScheduleService,
    IShowtimeQueryService showtimeQueryService
) : ControllerBase
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
    ) => Ok(await showtimeQueryService.GetShowtimesCalendarAsync(filter, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_ShowtimeCreated))]
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
    [SuccessMessage(nameof(Messages.Success_ShowtimeUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertShowtimeRequest request,
        CancellationToken cancellationToken
    ) => Ok(await showtimeService.UpdateShowtimeAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_ShowtimeDeleted))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        Ok(await showtimeCancellationService.DeleteShowtimeAsync(id, cancellationToken));

    [HttpPost("bulk")]
    [SuccessMessage(nameof(Messages.Success_ShowtimesBulkCreated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkCreate(
        [FromBody] BulkCreateShowtimesRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await showtimeBatchService.BulkCreateShowtimesAsync(request, cancellationToken)
        );

    [HttpPost("auto-schedule/preview")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> PreviewAutoSchedule(
        [FromBody] AutoScheduleRequest request,
        CancellationToken cancellationToken
    ) => Ok(await showtimeAutoScheduleService.PreviewAutoScheduleAsync(request, cancellationToken));

    [HttpPost("auto-schedule/apply")]
    [SuccessMessage(nameof(Messages.Success_ShowtimesAutoScheduleApplied))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> ApplyAutoSchedule(
        [FromBody] ApplyAutoScheduleRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await showtimeAutoScheduleService.ApplyAutoScheduleAsync(request, cancellationToken)
        );

    [HttpPost("clone")]
    [SuccessMessage(nameof(Messages.Success_ShowtimesCloned))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> CloneShowtimes(
        [FromBody] CloneShowtimesRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await showtimeBatchService.CloneShowtimesAsync(request, cancellationToken)
        );

    [HttpDelete("bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkDelete(
        [FromBody] BulkDeleteShowtimesRequest request,
        CancellationToken cancellationToken
    ) => Ok(await showtimeBatchService.BulkDeleteShowtimesAsync(request, cancellationToken));
}
