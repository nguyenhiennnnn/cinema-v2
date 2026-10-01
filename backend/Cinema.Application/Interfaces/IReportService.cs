using Cinema.Application.DTOs.Reports;

namespace Cinema.Application.Interfaces;

public interface IReportService
{
    Task<RevenueReportDto> GetRevenueReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        string groupBy,
        CancellationToken cancellationToken
    );

    Task<List<TopMovieDto>> GetTopMoviesReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        int limit,
        CancellationToken cancellationToken
    );

    Task<List<EmployeePerformanceDto>> GetEmployeePerformanceReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken
    );
}
