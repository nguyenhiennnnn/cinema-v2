using Cinema.Api.Filters;
using Cinema.Application.DTOs.Combos;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/combos")]
[Authorize]
public class AdminCombosController(IComboService comboService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> GetCombos(
        [FromQuery] int page = Pagination.DefaultPage,
        [FromQuery] int pageSize = Pagination.DefaultPageSize,
        CancellationToken cancellationToken = default
    ) => Ok(await comboService.GetCombosAdminAsync(page, pageSize, cancellationToken));

    [HttpPost]
    [SuccessMessage(nameof(Messages.Success_ComboCreated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Create(
        [FromBody] UpsertComboRequest request,
        CancellationToken cancellationToken
    ) =>
        StatusCode(
            StatusCodes.Status201Created,
            await comboService.CreateComboAsync(request, cancellationToken)
        );

    [HttpPut("{id:int}")]
    [SuccessMessage(nameof(Messages.Success_ComboUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertComboRequest request,
        CancellationToken cancellationToken
    ) => Ok(await comboService.UpdateComboAsync(id, request, cancellationToken));

    [HttpPut("{id:int}/toggle")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Toggle(int id, CancellationToken cancellationToken)
    {
        var result = await comboService.ToggleComboAsync(id, cancellationToken);
        return Ok(
            ApiResponse.SuccessResponse(
                result,
                result.IsActive ? Messages.Success_ComboActivated : Messages.Success_ComboDeactivated
            )
        );
    }

    [HttpPut("bulk/status")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> BulkSetActive(
        [FromBody] BulkStatusRequest request,
        CancellationToken cancellationToken
    ) => Ok(await comboService.SetCombosActiveAsync(request, cancellationToken));
}
