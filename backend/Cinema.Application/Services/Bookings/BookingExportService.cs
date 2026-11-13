using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Bookings;

public class BookingExportService(IAppDbContext db, IExportWriter exportWriter)
    : IBookingExportService
{
    private const string Module = "bookings";

    public async Task<ExportFileDto> ExportBookingsAsync(
        AdminBookingListFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var query = db.Bookings.AsNoTracking().ApplyAdminFilter(filter);
        await ExportLimits.EnsureWithinLimitAsync(query, cancellationToken);

        var rows = query
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.BookingId)
            .Take(ExportLimits.MaxRows)
            .Select(b => new BookingExportRow(
                b.BookingCode,
                b.CustomerName,
                b.CustomerPhone,
                b.Showtime.Movie.Title,
                b.Showtime.StartTime,
                b.Tickets.OrderBy(t => t.ShowtimeSeat.Seat.SeatRow)
                    .ThenBy(t => t.ShowtimeSeat.Seat.SeatNumber)
                    .Select(t => new SeatRef(
                        t.ShowtimeSeat.Seat.SeatRow,
                        t.ShowtimeSeat.Seat.SeatNumber
                    ))
                    .ToList(),
                b.BookingCombos.Select(bc => new ComboRef(bc.Combo.Name, bc.Quantity)).ToList(),
                b.TotalAmount,
                b.DiscountAmount,
                b.FinalAmount,
                b.Status,
                b.Channel,
                b.CreatedByEmployee == null ? null : b.CreatedByEmployee.User.FullName,
                b.CreatedAt
            ))
            .AsAsyncEnumerable();

        return await ExportFileBuilder.BuildAsync(
            exportWriter,
            Module,
            Messages.Export_SheetBookings,
            format,
            Columns(),
            rows,
            cancellationToken
        );
    }

    private static IReadOnlyList<ExportColumn<BookingExportRow>> Columns() =>
        [
            new(Messages.Export_ColBookingCode, x => x.BookingCode),
            new(Messages.Export_ColCustomer, x => x.CustomerName),
            new(Messages.Export_ColPhone, x => x.CustomerPhone),
            new(Messages.Export_ColMovieTitle, x => x.MovieTitle),
            new(Messages.Export_ColShowtime, x => x.ShowtimeStart, ExportColumnType.DateTime),
            new(
                Messages.Export_ColSeats,
                x => string.Join(", ", x.Seats.Select(s => $"{s.Row}{s.Number}"))
            ),
            new(
                Messages.Export_ColCombos,
                x => string.Join(", ", x.Combos.Select(c => $"{c.Name} x{c.Quantity}"))
            ),
            new(Messages.Export_ColTotalAmount, x => x.TotalAmount, ExportColumnType.Money),
            new(Messages.Export_ColDiscount, x => x.DiscountAmount, ExportColumnType.Money),
            new(Messages.Export_ColFinalAmount, x => x.FinalAmount, ExportColumnType.Money),
            new(Messages.Export_ColStatus, x => ExportLabels.Booking(x.Status)),
            new(Messages.Export_ColChannel, x => ExportLabels.Channel(x.Channel)),
            new(Messages.Export_ColCreatedBy, x => x.CreatedBy),
            new(Messages.Export_ColCreatedAt, x => x.CreatedAt, ExportColumnType.DateTime),
        ];

    private sealed record SeatRef(string Row, int Number);

    private sealed record ComboRef(string Name, int Quantity);

    private sealed record BookingExportRow(
        string BookingCode,
        string CustomerName,
        string? CustomerPhone,
        string MovieTitle,
        DateTime ShowtimeStart,
        List<SeatRef> Seats,
        List<ComboRef> Combos,
        decimal TotalAmount,
        decimal DiscountAmount,
        decimal FinalAmount,
        BookingStatus Status,
        BookingChannel Channel,
        string? CreatedBy,
        DateTime CreatedAt
    );
}
