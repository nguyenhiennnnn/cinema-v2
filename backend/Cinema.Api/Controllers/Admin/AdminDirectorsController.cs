using Cinema.Api.Filters;
using Cinema.Application.DTOs.Catalog;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/directors")]
[Authorize]
public class AdminDirectorsController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await catalogService.GetDirectorsAdminAsync(search, page, pageSize, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_DirectorCreated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertPersonRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreateDirectorAsync(request, cancellationToken)
        );

    [HttpPost("quick")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> QuickCreate(
        [FromBody] QuickCreatePersonRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.QuickCreateDirectorAsync(request, cancellationToken));

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_DirectorUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertPersonRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdateDirectorAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_DirectorDeleted))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeleteDirectorAsync(id, cancellationToken);
        return Ok(new { directorId = result.Id, deleted = result.Deleted });
    }

    [HttpDelete("bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkDelete(
        [FromBody] BulkIdsRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.BulkDeleteDirectorsAsync(request, cancellationToken));
}
