using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/seat-types")]
[Authorize]
public class AdminSeatTypesController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await catalogService.GetSeatTypesAsync(page, pageSize, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertSeatTypeRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreateSeatTypeAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertSeatTypeRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdateSeatTypeAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeleteSeatTypeAsync(id, cancellationToken);
        return Ok(new { seatTypeId = result.Id, deleted = result.Deleted });
    }
}
