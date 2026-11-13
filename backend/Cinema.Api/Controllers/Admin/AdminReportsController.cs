using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/reports")]
[Authorize(Policy = Policies.AdminOnly)]
public class AdminReportsController(
    IReportService reportService,
    IReportExportService reportExportService
) : ControllerBase
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
        var effectiveLimit = ResolveLimit(limit);
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

    [HttpGet("revenue/export")]
    public async Task<IActionResult> ExportRevenue(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] string groupBy,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    )
    {
        var exportFormat = ExportFormats.Parse(format);
        var file = await reportExportService.ExportRevenueAsync(
            fromDate,
            toDate,
            groupBy,
            exportFormat,
            cancellationToken
        );
        return ToFileResult(file);
    }

    [HttpGet("top-movies/export")]
    public async Task<IActionResult> ExportTopMovies(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] int? limit,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    )
    {
        var exportFormat = ExportFormats.Parse(format);
        var file = await reportExportService.ExportTopMoviesAsync(
            fromDate,
            toDate,
            ResolveLimit(limit),
            exportFormat,
            cancellationToken
        );
        return ToFileResult(file);
    }

    [HttpGet("employee-performance/export")]
    public async Task<IActionResult> ExportEmployeePerformance(
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] string? format,
        CancellationToken cancellationToken
    )
    {
        var exportFormat = ExportFormats.Parse(format);
        var file = await reportExportService.ExportEmployeePerformanceAsync(
            fromDate,
            toDate,
            exportFormat,
            cancellationToken
        );
        return ToFileResult(file);
    }

    private static int ResolveLimit(int? limit) =>
        Math.Clamp(limit ?? Pagination.DefaultPageSize, 1, 100);

    private FileStreamResult ToFileResult(ExportFileDto file) =>
        File(file.Content, file.ContentType, file.FileName);
}
