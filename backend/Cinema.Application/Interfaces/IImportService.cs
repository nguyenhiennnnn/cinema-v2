using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Import;

namespace Cinema.Application.Interfaces;

public interface IImportService<TRow>
{
    Task<ExportFileDto> GetTemplateAsync(CancellationToken cancellationToken);

    Task<ImportPreviewResponse> PreviewAsync(
        int userId,
        string? fileName,
        Stream content,
        CancellationToken cancellationToken
    );

    Task<ImportApplyOutcome> ApplyAsync(
        int userId,
        ImportApplyRequest request,
        CancellationToken cancellationToken
    );

    Task<BulkActionResponse> RunQueuedApplyAsync(
        int userId,
        string previewId,
        bool skipInvalidRows,
        IJobProgressReporter progress,
        CancellationToken cancellationToken
    );

    Task<ExportFileDto> GetErrorFileAsync(
        int userId,
        string previewId,
        CancellationToken cancellationToken
    );
}
