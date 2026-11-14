using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.ImportExport;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.BackgroundJobs;

public class ImportApplyJob(
    IEnumerable<IImportApplyModule> modules,
    IImportExportJobTracker tracker,
    IOptions<ImportExportOptions> options,
    ILogger<ImportApplyJob> logger
)
{
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(
        Guid jobId,
        string module,
        string previewId,
        bool skipInvalidRows,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var context = await tracker.MarkRunningAsync(jobId, cancellationToken);
            if (context is null)
            {
                logger.LogWarning("ImportApplyJob: JobId {JobId} không tồn tại.", jobId);
                return;
            }

            var handler =
                modules.FirstOrDefault(m => m.Module == module)
                ?? throw new InvalidOperationException(
                    $"Module nhập dữ liệu '{module}' chưa được đăng ký."
                );

            var progress = new JobProgressReporter(
                tracker,
                jobId,
                context.TotalRows,
                options.Value.ImportProgressBatchSize
            );

            var result = await handler.ApplyQueuedAsync(
                context.UserId,
                previewId,
                skipInvalidRows,
                progress,
                cancellationToken
            );

            var errorMessage = result
                .Results.Where(item => !item.Success)
                .Select(item => item.Reason)
                .FirstOrDefault(reason => !string.IsNullOrWhiteSpace(reason));

            await tracker.CompleteImportAsync(
                jobId,
                result.TotalSuccess,
                result.TotalFailed,
                errorMessage,
                CancellationToken.None
            );
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "ImportApplyJob thất bại JobId {JobId} Module {Module}", jobId, module);
            await tracker.FailAsync(
                jobId,
                ImportExportJobErrors.Describe(ex),
                CancellationToken.None
            );
            throw;
        }
    }
}
