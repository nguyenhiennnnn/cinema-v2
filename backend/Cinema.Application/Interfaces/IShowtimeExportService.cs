using Cinema.Application.DTOs.Common;
using Cinema.Application.Utils;

namespace Cinema.Application.Interfaces;

public interface IShowtimeExportService
{
    Task<ExportFileDto> ExportShowtimesAsync(
        AdminShowtimeListFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    );
}
