using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;

namespace Cinema.Application.Services.ImportExport;

public class ImportApplyModule<TRow>(
    IImportService<TRow> importService,
    IImportProcessor<TRow> processor
) : IImportApplyModule
{
    public string Module => processor.Module;

    public Task<BulkActionResponse> ApplyQueuedAsync(
        int userId,
        string previewId,
        bool skipInvalidRows,
        IJobProgressReporter progress,
        CancellationToken cancellationToken
    ) =>
        importService.RunQueuedApplyAsync(
            userId,
            previewId,
            skipInvalidRows,
            progress,
            cancellationToken
        );
}
