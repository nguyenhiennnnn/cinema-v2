using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.ImportExport;
using Cinema.Application.Utils;

namespace Cinema.Application.Interfaces;

public interface IExportJobService
{
    Task<ExportFileDto> ExportNowAsync<TFilter>(
        int userId,
        string module,
        TFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    );

    Task<ImportExportJobAcceptedDto> EnqueueAsync<TFilter>(
        int userId,
        string module,
        TFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    );
}
