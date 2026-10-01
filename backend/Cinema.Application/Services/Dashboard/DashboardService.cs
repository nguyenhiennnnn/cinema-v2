using Cinema.Application.DTOs.Dashboard;
using Cinema.Application.Interfaces;
using Cinema.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Dashboard;

public class DashboardService(IAppDbContext db, IReportService reportService) : IDashboardService
{
    private const int TopMoviesLimit = 5;

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);
        var yesterdayStart = todayStart.AddDays(-1);

        var todayRevenue =
            await db
                .Bookings.Where(b =>
                    b.Status == BookingStatus.Confirmed
                    && b.CreatedAt >= todayStart
                    && b.CreatedAt < todayEnd
                )
                .SumAsync(b => (decimal?)b.FinalAmount, cancellationToken)
            ?? 0m;

        var yesterdayRevenue =
            await db
                .Bookings.Where(b =>
                    b.Status == BookingStatus.Confirmed
                    && b.CreatedAt >= yesterdayStart
                    && b.CreatedAt < todayStart
                )
                .SumAsync(b => (decimal?)b.FinalAmount, cancellationToken)
            ?? 0m;

        decimal? revenueChangePercent =
            yesterdayRevenue > 0
                ? Math.Round(
                    (todayRevenue - yesterdayRevenue) / yesterdayRevenue * 100m,
                    1,
                    MidpointRounding.AwayFromZero
                )
                : null;

        var ticketsSoldToday = await db.BookingTickets.CountAsync(
            t =>
                t.Booking.Status == BookingStatus.Confirmed
                && t.Booking.CreatedAt >= todayStart
                && t.Booking.CreatedAt < todayEnd,
            cancellationToken
        );

        var confirmedBookingsToday = await db.Bookings.CountAsync(
            b =>
                b.Status == BookingStatus.Confirmed
                && b.CreatedAt >= todayStart
                && b.CreatedAt < todayEnd,
            cancellationToken
        );

        var activePendingBookings = await db.Bookings.CountAsync(
            b => b.Status == BookingStatus.Pending && b.ExpiresAt != null && b.ExpiresAt > now,
            cancellationToken
        );

        var todaySeatCounts = await db
            .ShowtimeSeats.Where(ss =>
                ss.Showtime.StartTime >= todayStart && ss.Showtime.StartTime < todayEnd
            )
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Booked = g.Count(ss => ss.Status == ShowtimeSeatStatus.Booked),
            })
            .FirstOrDefaultAsync(cancellationToken);

        decimal? todayOccupancyPercent = todaySeatCounts is { Total: > 0 }
            ? Math.Round(
                (decimal)todaySeatCounts.Booked / todaySeatCounts.Total * 100m,
                1,
                MidpointRounding.AwayFromZero
            )
            : null;

        return new DashboardSummaryDto(
            todayRevenue,
            yesterdayRevenue,
            revenueChangePercent,
            ticketsSoldToday,
            confirmedBookingsToday,
            activePendingBookings,
            todayOccupancyPercent
        );
    }

    public async Task<DashboardInsightsDto> GetInsightsAsync(
        int days,
        CancellationToken cancellationToken
    )
    {
        var clampedDays = Math.Clamp(days, 1, 90);

        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);

        var toDate = DateOnly.FromDateTime(todayStart);
        var fromDate = toDate.AddDays(-(clampedDays - 1));

        var revenueReport = await reportService.GetRevenueReportAsync(
            fromDate,
            toDate,
            "day",
            cancellationToken
        );
        var topMovies = await reportService.GetTopMoviesReportAsync(
            fromDate,
            toDate,
            TopMoviesLimit,
            cancellationToken
        );

        var todayShowtimes = await db
            .Showtimes.Where(s => s.StartTime >= todayStart && s.StartTime < todayEnd)
            .OrderBy(s => s.StartTime)
            .ProjectToType<DashboardShowtimeItemDto>()
            .ToListAsync(cancellationToken);

        return new DashboardInsightsDto(revenueReport.Items, todayShowtimes, topMovies);
    }
}
