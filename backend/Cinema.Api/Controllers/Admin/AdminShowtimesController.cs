using Cinema.Api.Filters;
using Cinema.Application.DTOs.Import;
using Cinema.Application.DTOs.ImportExport;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
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
    IShowtimeQueryService showtimeQueryService,
    IExportJobService exportJobService,
    IImportService<ShowtimeImportRow> showtimeImportService
) : AppControllerBase
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

    [HttpGet("export")]
    [Authorize(Policy = Policies.StaffOnly)]
    public async Task<IActionResult> ExportShowtimes(
        [FromQuery] AdminShowtimeListFilter filter,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    )
    {
        var exportFormat = ExportFormats.Parse(format);
        var file = await exportJobService.ExportNowAsync(
            CurrentUserId,
            ImportExportModules.Showtimes,
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
        [FromQuery] AdminShowtimeListFilter filter,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status202Accepted,
            await exportJobService.EnqueueAsync(
                CurrentUserId,
                ImportExportModules.Showtimes,
                filter,
                ExportFormats.Parse(format),
                cancellationToken
            )
        );

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

    [HttpGet("import/template")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> DownloadImportTemplate(CancellationToken cancellationToken)
    {
        var file = await showtimeImportService.GetTemplateAsync(cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost("import/preview")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(ImportLimits.MaxRequestBytes)]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> PreviewImport(
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
        var result = await showtimeImportService.PreviewAsync(
            CurrentUserId,
            file.FileName,
            stream,
            cancellationToken
        );

        return Ok(result);
    }

    [HttpPost("import/apply")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> ApplyImport(
        [FromBody] ImportApplyRequest request,
        CancellationToken cancellationToken
    )
    {
        var outcome = await showtimeImportService.ApplyAsync(
            CurrentUserId,
            request,
            cancellationToken
        );
        return outcome.JobId is { } jobId
            ? StatusCode(StatusCodes.Status202Accepted, new ImportExportJobAcceptedDto(jobId))
            : Ok(outcome.Result);
    }

    [HttpGet("import/errors/{previewId}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> DownloadImportErrors(
        string previewId,
        CancellationToken cancellationToken
    )
    {
        var file = await showtimeImportService.GetErrorFileAsync(
            CurrentUserId,
            previewId,
            cancellationToken
        );
        return File(file.Content, file.ContentType, file.FileName);
    }
}
