using Cinema.Domain.Enums;

namespace Cinema.Application.Interfaces;

public sealed record ImportExportJobRunContext(int UserId, int TotalRows);

public interface IImportExportJobTracker
{
    Task<Guid> CreateQueuedAsync(
        ImportExportJobType type,
        string module,
        string format,
        int userId,
        string? fileName,
        int totalRows,
        CancellationToken cancellationToken
    );

    Task<ImportExportJobRunContext?> MarkRunningAsync(
        Guid jobId,
        CancellationToken cancellationToken
    );

    Task ReportProgressAsync(Guid jobId, int progress, CancellationToken cancellationToken);

    Task CompleteExportAsync(
        Guid jobId,
        string fileName,
        FileUploadResult file,
        int totalRows,
        int retentionHours,
        CancellationToken cancellationToken
    );

    Task CompleteImportAsync(
        Guid jobId,
        int successRows,
        int failedRows,
        string? errorMessage,
        CancellationToken cancellationToken
    );

    Task FailAsync(Guid jobId, string errorMessage, CancellationToken cancellationToken);

    Task RecordCompletedAsync(
        ImportExportJobType type,
        string module,
        string format,
        int userId,
        string? fileName,
        int totalRows,
        int successRows,
        int failedRows,
        CancellationToken cancellationToken
    );
}
