using Cinema.Api.Filters;
using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/positions")]
[Authorize(Policy = Policies.AdminOnly)]
public class AdminPositionsController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await catalogService.GetPositionsAsync(page, pageSize, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_PositionCreated))]
    public async Task<IActionResult> Create(
        [FromBody] UpsertPositionRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreatePositionAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_PositionUpdated))]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertPositionRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdatePositionAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_PositionDeleted))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeletePositionAsync(id, cancellationToken);
        return Ok(new { positionId = result.Id, deleted = result.Deleted });
    }
}
