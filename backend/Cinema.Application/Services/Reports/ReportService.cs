using Cinema.Application.DTOs.Reports;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
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
                Messages.Invalid_GroupBy,
                new Dictionary<string, string[]> { ["groupBy"] = [Messages.Invalid_GroupBy] }
            );

        var fromDateVal = DateTimeUtils.GetStartOfDay(fromDate);
        var toDateVal = DateTimeUtils.GetStartOfNextDay(toDate);

        var payments = await db
            .Payments.Where(p =>
                (p.Status == PaymentStatus.Success || p.Status == PaymentStatus.Refunded)
                && p.PaidAt != null
                && p.PaidAt >= fromDateVal
                && p.PaidAt < toDateVal
            )
            .Select(p => new
            {
                p.BookingId,
                PaidAt = p.PaidAt!.Value,
                p.Amount,
                p.RefundedAmount,
                BookingFinalAmount = p.Booking.FinalAmount,
                TicketRevenue = p.Booking.Tickets.Sum(t => (decimal?)t.Price) ?? 0m,
                ComboRevenue = p.Booking.BookingCombos.Sum(bc =>
                    (decimal?)(bc.Quantity * bc.UnitPrice)
                ) ?? 0m,
            })
            .ToListAsync(cancellationToken);

        var rows = payments
            .Select(p =>
            {
                var net = Math.Max(0m, p.Amount - (p.RefundedAmount ?? 0m));
                var ratio =
                    p.BookingFinalAmount > 0m ? Math.Min(1m, net / p.BookingFinalAmount) : 0m;
                return new
                {
                    p.BookingId,
                    p.PaidAt,
                    Net = net,
                    TicketRevenue = p.TicketRevenue * ratio,
                    ComboRevenue = p.ComboRevenue * ratio,
                };
            })
            .Where(r => r.Net > 0m)
            .ToList();

        var items = rows.GroupBy(r => DateTimeUtils.GetPeriodKey(r.PaidAt, groupBy))
            .OrderBy(g => g.Min(x => x.PaidAt))
            .Select(g => new
            {
                Period = g.Key,
                TicketRevenue = g.Sum(x => x.TicketRevenue),
                ComboRevenue = g.Sum(x => x.ComboRevenue),
                TotalRevenue = g.Sum(x => x.Net),
                BookingCount = g.Select(x => x.BookingId).Distinct().Count(),
            })
            .Adapt<List<RevenueReportItemDto>>();

        var summary = new
        {
            TotalRevenue = rows.Sum(r => r.Net),
            TotalBookings = rows.Select(r => r.BookingId).Distinct().Count(),
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
