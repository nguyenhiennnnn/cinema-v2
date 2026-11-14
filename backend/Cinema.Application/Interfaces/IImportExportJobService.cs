using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.ImportExport;

namespace Cinema.Application.Interfaces;

public interface IImportExportJobService
{
    Task<OffsetPageResult<ImportExportJobDto>> GetJobsAsync(
        int userId,
        ImportExportJobListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );

    Task<ImportExportJobDto> GetJobAsync(
        int userId,
        Guid jobId,
        CancellationToken cancellationToken
    );

    Task<ExportFileDto> DownloadAsync(
        int userId,
        Guid jobId,
        CancellationToken cancellationToken
    );

    Task<int> CleanupExpiredFilesAsync(CancellationToken cancellationToken);
}
