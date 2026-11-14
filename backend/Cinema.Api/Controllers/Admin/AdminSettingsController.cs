using Cinema.Api.Filters;
using Cinema.Application.DTOs.Settings;
using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/settings")]
[Authorize]
public class AdminSettingsController(ISettingsService settingsService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
        Ok(await settingsService.GetAdminSettingsAsync(cancellationToken));

    [HttpPut]
    [SuccessMessage(nameof(Messages.Success_SettingsUpdated))]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<IActionResult> Update(
        [FromBody] UpdateSettingsRequest request,
        CancellationToken cancellationToken
    ) => Ok(await settingsService.UpdateSettingsAsync(request, cancellationToken));
}
