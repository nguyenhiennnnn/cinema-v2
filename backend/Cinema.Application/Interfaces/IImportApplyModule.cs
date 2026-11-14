using Cinema.Application.DTOs.Common;

namespace Cinema.Application.Interfaces;

public interface IImportApplyModule
{
    string Module { get; }

    Task<BulkActionResponse> ApplyQueuedAsync(
        int userId,
        string previewId,
        bool skipInvalidRows,
        IJobProgressReporter progress,
        CancellationToken cancellationToken
    );
}
