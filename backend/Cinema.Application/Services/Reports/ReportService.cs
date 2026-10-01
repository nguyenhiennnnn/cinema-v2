using Cinema.Application.DTOs.Reports;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Reports;

public class ReportService(IAppDbContext db) : IReportService
{
    private static readonly string[] ValidGroupBy = ["day", "week", "month"];

    public async Task<RevenueReportDto> GetRevenueReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        string groupBy,
        CancellationToken cancellationToken
    )
    {
        if (!ValidGroupBy.Contains(groupBy))
            throw new ValidationAppException(
                "groupBy phải là day, week hoặc month.",
                new Dictionary<string, string[]>
                {
                    ["groupBy"] = ["groupBy phải là day, week hoặc month."],
                }
            );

        var fromDateVal = DateTimeUtils.GetStartOfDay(fromDate);
        var toDateVal = DateTimeUtils.GetStartOfNextDay(toDate);

        var bookings = await db
            .Bookings.Where(b =>
                b.Status == BookingStatus.Confirmed
                && b.CreatedAt >= fromDateVal
                && b.CreatedAt < toDateVal
            )
            .Select(b => new
            {
                b.CreatedAt,
                b.FinalAmount,
                TicketRevenue = b.Tickets.Sum(t => (decimal?)t.Price) ?? 0m,
                ComboRevenue = b.BookingCombos.Sum(bc => (decimal?)(bc.Quantity * bc.UnitPrice))
                    ?? 0m,
            })
            .ToListAsync(cancellationToken);

        var items = bookings
            .GroupBy(b => DateTimeUtils.GetPeriodKey(b.CreatedAt, groupBy))
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => new
            {
                Period = g.Key,
                TicketRevenue = g.Sum(x => x.TicketRevenue),
                ComboRevenue = g.Sum(x => x.ComboRevenue),
                TotalRevenue = g.Sum(x => x.FinalAmount),
                BookingCount = g.Count(),
            })
            .Adapt<List<RevenueReportItemDto>>();

        var summary = new
        {
            TotalRevenue = bookings.Sum(b => b.FinalAmount),
            TotalBookings = bookings.Count,
        }.Adapt<RevenueSummaryDto>();

        return new RevenueReportDto(groupBy, items, summary);
    }

    public async Task<List<TopMovieDto>> GetTopMoviesReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        int limit,
        CancellationToken cancellationToken
    )
    {
        var fromDateVal = DateTimeUtils.GetStartOfDay(fromDate);
        var toDateVal = DateTimeUtils.GetStartOfNextDay(toDate);

        var query = db
            .BookingTickets.Where(t =>
                t.Booking.Status == BookingStatus.Confirmed
                && t.Booking.CreatedAt >= fromDateVal
                && t.Booking.CreatedAt < toDateVal
            )
            .GroupBy(t => new
            {
                t.ShowtimeSeat.Showtime.MovieId,
                t.ShowtimeSeat.Showtime.Movie.Title,
            })
            .Select(g => new
            {
                g.Key.MovieId,
                g.Key.Title,
                TicketsSold = g.Count(),
                Revenue = g.Sum(t => t.Price),
            })
            .OrderByDescending(x => x.Revenue)
            .Take(limit);

        var results = await query.ToListAsync(cancellationToken);
        return results.Adapt<List<TopMovieDto>>();
    }

    public async Task<List<EmployeePerformanceDto>> GetEmployeePerformanceReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken
    )
    {
        var fromDateVal = DateTimeUtils.GetStartOfDay(fromDate);
        var toDateVal = DateTimeUtils.GetStartOfNextDay(toDate);

        var query = db
            .Bookings.Where(b =>
                b.Status == BookingStatus.Confirmed
                && b.Channel == BookingChannel.Counter
                && b.CreatedByEmployeeId != null
                && b.CreatedAt >= fromDateVal
                && b.CreatedAt < toDateVal
            )
            .GroupBy(b => new { b.CreatedByEmployeeId, b.CreatedByEmployee!.User.FullName })
            .Select(g => new
            {
                EmployeeId = g.Key.CreatedByEmployeeId!.Value,
                g.Key.FullName,
                TicketsSold = g.SelectMany(b => b.Tickets).Count(),
                Revenue = g.Sum(b => b.FinalAmount),
                BookingCount = g.Count(),
            })
            .OrderByDescending(x => x.Revenue);

        var results = await query.ToListAsync(cancellationToken);
        return results.Adapt<List<EmployeePerformanceDto>>();
    }
}
