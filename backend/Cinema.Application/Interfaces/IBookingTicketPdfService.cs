using Cinema.Application.DTOs.Common;

namespace Cinema.Application.Interfaces;

public interface IBookingTicketPdfService
{
    Task<ExportFileDto> GenerateForCustomerAsync(
        int userId,
        int bookingId,
        CancellationToken cancellationToken
    );

    Task<ExportFileDto> GenerateForStaffAsync(
        int bookingId,
        CancellationToken cancellationToken
    );
}
