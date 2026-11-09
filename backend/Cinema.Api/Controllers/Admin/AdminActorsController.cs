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
[Route("api/v1/admin/actors")]
[Authorize]
public class AdminActorsController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await catalogService.GetActorsAdminAsync(search, page, pageSize, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_ActorCreated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertPersonRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreateActorAsync(request, cancellationToken)
        );

    [HttpPost("quick")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> QuickCreate(
        [FromBody] QuickCreatePersonRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.QuickCreateActorAsync(request, cancellationToken));

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_ActorUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertPersonRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdateActorAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_ActorDeleted))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeleteActorAsync(id, cancellationToken);
        return Ok(new { actorId = result.Id, deleted = result.Deleted });
    }

    [HttpDelete("bulk")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkDelete(
        [FromBody] BulkIdsRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.BulkDeleteActorsAsync(request, cancellationToken));
}
