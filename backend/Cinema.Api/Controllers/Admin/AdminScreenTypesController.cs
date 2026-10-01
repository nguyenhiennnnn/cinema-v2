using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/screen-types")]
[Authorize]
public class AdminScreenTypesController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await catalogService.GetScreenTypesAsync(page, pageSize, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertScreenTypeRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreateScreenTypeAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertScreenTypeRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdateScreenTypeAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeleteScreenTypeAsync(id, cancellationToken);
        return Ok(new { screenTypeId = result.Id, deleted = result.Deleted });
    }
}
