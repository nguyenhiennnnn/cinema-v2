using Cinema.Application.DTOs.Common;
using Cinema.Application.Utils;

namespace Cinema.Application.Interfaces;

public interface IReportExportService
{
    Task<ExportFileDto> ExportRevenueAsync(
        DateOnly fromDate,
        DateOnly toDate,
        string groupBy,
        ExportFormat format,
        CancellationToken cancellationToken
    );

    Task<ExportFileDto> ExportTopMoviesAsync(
        DateOnly fromDate,
        DateOnly toDate,
        int limit,
        ExportFormat format,
        CancellationToken cancellationToken
    );

    Task<ExportFileDto> ExportEmployeePerformanceAsync(
        DateOnly fromDate,
        DateOnly toDate,
        ExportFormat format,
        CancellationToken cancellationToken
    );
}
