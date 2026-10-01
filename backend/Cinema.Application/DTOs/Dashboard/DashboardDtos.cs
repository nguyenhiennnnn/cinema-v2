namespace Cinema.Application.DTOs.Dashboard;

public record DashboardSummaryDto(
    decimal TodayRevenue,
    decimal YesterdayRevenue,
    decimal? RevenueChangePercent,
    int TicketsSoldToday,
    int ConfirmedBookingsToday,
    int ActivePendingBookings,
    decimal? TodayOccupancyPercent
);

public record DashboardShowtimeItemDto(
    int ShowtimeId,
    string MovieTitle,
    string RoomName,
    DateTime StartTime,
    int BookedSeats,
    int TotalSeats
);

public record DashboardInsightsDto(
    List<Reports.RevenueReportItemDto> RevenueTrend,
    List<DashboardShowtimeItemDto> TodayShowtimes,
    List<Reports.TopMovieDto> TopMovies
);
