using Cinema.Api.Filters;
using Cinema.Application.DTOs.Catalog;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/membership-tiers")]
[Authorize(Policy = Policies.AdminOnly)]
public class AdminMembershipTiersController(ICatalogService catalogService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await catalogService.GetMembershipTiersAdminAsync(page, pageSize, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_MembershipTierCreated))]
    public async Task<IActionResult> Create(
        [FromBody] UpsertMembershipTierRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await catalogService.CreateMembershipTierAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_MembershipTierUpdated))]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertMembershipTierRequest request,
        CancellationToken cancellationToken
    ) => Ok(await catalogService.UpdateMembershipTierAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_MembershipTierDeleted))]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await catalogService.DeleteMembershipTierAsync(id, cancellationToken);
        return Ok(new { tierId = result.Id, deleted = result.Deleted });
    }
}
