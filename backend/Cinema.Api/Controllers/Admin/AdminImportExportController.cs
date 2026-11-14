using Cinema.Application.DTOs.ImportExport;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/import-export")]
[Authorize]
public class AdminImportExportController(IImportExportJobService jobService) : AppControllerBase
{
    [HttpGet("jobs")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetJobs(
        [FromQuery] ImportExportJobListFilter filter,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await jobService.GetJobsAsync(CurrentUserId, filter, page, pageSize, cancellationToken));

    [HttpGet("jobs/{id:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetJob(Guid id, CancellationToken cancellationToken) =>
        Ok(await jobService.GetJobAsync(CurrentUserId, id, cancellationToken));

    [HttpGet("jobs/{id:guid}/download")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> DownloadJobFile(Guid id, CancellationToken cancellationToken)
    {
        var file = await jobService.DownloadAsync(CurrentUserId, id, cancellationToken);
        return File(file.Content, file.ContentType, file.FileName);
    }
}
