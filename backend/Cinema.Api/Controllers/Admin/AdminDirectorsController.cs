using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
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
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertPersonRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreateDirectorAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertPersonRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdateDirectorAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeleteDirectorAsync(id, cancellationToken);
        return Ok(new { directorId = result.Id, deleted = result.Deleted });
    }
}
