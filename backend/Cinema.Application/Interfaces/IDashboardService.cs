using Cinema.Application.DTOs.Dashboard;

namespace Cinema.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken);

    Task<DashboardInsightsDto> GetInsightsAsync(int days, CancellationToken cancellationToken);
}
