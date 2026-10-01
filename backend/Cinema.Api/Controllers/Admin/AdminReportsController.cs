using Cinema.Application.Interfaces;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/reports")]
[Authorize(Policy = Policies.AdminOnly)]
public class AdminReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] string groupBy,
        CancellationToken cancellationToken
    )
    {
        var result = await reportService.GetRevenueReportAsync(
            fromDate,
            toDate,
            groupBy,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("top-movies")]
    public async Task<IActionResult> GetTopMovies(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] int? limit,
        CancellationToken cancellationToken
    )
    {
        var effectiveLimit = Math.Clamp(limit ?? Pagination.DefaultPageSize, 1, 100);
        var result = await reportService.GetTopMoviesReportAsync(
            fromDate,
            toDate,
            effectiveLimit,
            cancellationToken
        );
        return Ok(result);
    }

    [HttpGet("employee-performance")]
    public async Task<IActionResult> GetEmployeePerformance(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        CancellationToken cancellationToken
    )
    {
        var result = await reportService.GetEmployeePerformanceReportAsync(
            fromDate,
            toDate,
            cancellationToken
        );
        return Ok(result);
    }
}
