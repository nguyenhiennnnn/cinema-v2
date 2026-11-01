using Cinema.Api.Filters;
using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/age-ratings")]
[Authorize]
public class AdminAgeRatingsController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await catalogService.GetAgeRatingsAdminAsync(page, pageSize, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_AgeRatingCreated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertAgeRatingRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreateAgeRatingAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_AgeRatingUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertAgeRatingRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdateAgeRatingAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_AgeRatingDeleted))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeleteAgeRatingAsync(id, cancellationToken);
        return Ok(new { ageRatingId = result.Id, deleted = result.Deleted });
    }
}
