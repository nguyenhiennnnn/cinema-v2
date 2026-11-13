using Cinema.Application.DTOs.Common;
using Cinema.Application.Utils;

namespace Cinema.Application.Interfaces;

public interface IBookingExportService
{
    Task<ExportFileDto> ExportBookingsAsync(
        AdminBookingListFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    );
}
