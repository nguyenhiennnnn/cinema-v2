using Cinema.Application.DTOs.Combos;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
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
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertComboRequest request,
        CancellationToken cancellationToken
    ) => Ok(await comboService.UpdateComboAsync(id, request, cancellationToken));

    [HttpPut("{id:int}/toggle")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Toggle(int id, CancellationToken cancellationToken) =>
        Ok(await comboService.ToggleComboAsync(id, cancellationToken));
}
