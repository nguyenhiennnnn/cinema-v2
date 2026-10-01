using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/dashboard")]
[Authorize(Policy = Policies.AdminOnly)]
public class AdminDashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await dashboardService.GetSummaryAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("insights")]
    public async Task<IActionResult> GetInsights(
        [FromQuery] int days,
        CancellationToken cancellationToken
    )
    {
        var effectiveDays = days <= 0 ? 7 : days;
        var result = await dashboardService.GetInsightsAsync(effectiveDays, cancellationToken);
        return Ok(result);
    }
}
