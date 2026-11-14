using System.Globalization;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Pdf;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Bookings;

public class BookingTicketPdfService(
    IAppDbContext db,
    IPdfGenerator pdfGenerator,
    IOptions<BrandOptions> brandOptions
) : IBookingTicketPdfService
{
    public Task<ExportFileDto> GenerateForCustomerAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    ) => BuildAsync(bookingId, userId, cancellationToken);

    public Task<ExportFileDto> GenerateForStaffAsync(
        int bookingId,
        CancellationToken cancellationToken
    ) => BuildAsync(bookingId, null, cancellationToken);

    private async Task<ExportFileDto> BuildAsync(
        int bookingId,
        int? ownerUserId,
        CancellationToken cancellationToken
    )
    {
        var booking =
            await db
                .Bookings.AsNoTracking()
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(b => b.Showtime)
                    .ThenInclude(s => s.Room)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_Booking);

        if (ownerUserId is not null && booking.UserId != ownerUserId)
            throw new ForbiddenAppException(Messages.Forbidden_NotBookingOwner);

        if (booking.Status != BookingStatus.Confirmed)
            throw new BusinessRuleException(Messages.Rule_TicketPdfNotPaid);

        var tickets = await db
            .BookingTickets.AsNoTracking()
            .Where(t => t.BookingId == bookingId)
            .OrderBy(t => t.ShowtimeSeat.Seat.SeatRow)
            .ThenBy(t => t.ShowtimeSeat.Seat.SeatNumber)
            .Select(t => new
            {
                t.ShowtimeSeat.Seat.SeatRow,
                t.ShowtimeSeat.Seat.SeatNumber,
                t.QrCode,
            })
            .ToListAsync(cancellationToken);

        if (tickets.Count == 0)
            throw new BusinessRuleException(Messages.Rule_TicketPdfNotPaid);

        var settings = await db.LoadCinemaSettingsOrDefaultAsync(cancellationToken);
        var cinemaName = string.IsNullOrWhiteSpace(settings.Name)
            ? brandOptions.Value.Name
            : settings.Name;

        var items = tickets
            .Select(t =>
            {
                var payload =
                    t.QrCode
                    ?? CodeGenerator.GenerateTicketQrPayload(
                        booking.BookingCode,
                        t.SeatRow,
                        t.SeatNumber
                    );
                return new PdfTicketItem(
                    $"{t.SeatRow}{t.SeatNumber}",
                    QrCodeHelper.GeneratePngBytes(payload)
                );
            })
            .ToList();

        var document = new PdfTicketDocument(
            cinemaName,
            booking.Showtime.Movie.Title,
            BusinessTime
                .ToLocal(booking.Showtime.StartTime)
                .ToString("HH:mm dd/MM/yyyy", CultureInfo.InvariantCulture),
            booking.Showtime.Room.RoomName,
            booking.BookingCode,
            new PdfTicketLabels(
                Messages.Export_PdfTicketTitle,
                Messages.Export_PdfTicketMovie,
                Messages.Export_PdfTicketShowtime,
                Messages.Export_PdfTicketRoom,
                Messages.Export_PdfTicketSeat,
                Messages.Export_PdfTicketBookingCode,
                Messages.Export_PdfTicketHint
            ),
            items
        );

        return new ExportFileDto(
            pdfGenerator.GenerateTickets(document),
            ExportFormats.ContentType(ExportFormat.Pdf),
            $"tickets-{booking.BookingCode}.{ExportFormats.Extension(ExportFormat.Pdf)}"
        );
    }
}
