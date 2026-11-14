using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.ImportExport;
using Cinema.Application.Utils;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.BackgroundJobs;

public class ExportJob(
    IEnumerable<IExportModule> modules,
    IImportExportJobTracker tracker,
    IFileStorage fileStorage,
    IOptions<ImportExportOptions> options,
    ILogger<ExportJob> logger
)
{
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync(
        Guid jobId,
        string module,
        string filterJson,
        string format,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var context = await tracker.MarkRunningAsync(jobId, cancellationToken);
            if (context is null)
            {
                logger.LogWarning("ExportJob: JobId {JobId} không tồn tại.", jobId);
                return;
            }

            var handler =
                modules.FirstOrDefault(m => m.Module == module)
                ?? throw new InvalidOperationException(
                    $"Module xuất dữ liệu '{module}' chưa được đăng ký."
                );

            var settings = options.Value;
            var progress = new JobProgressReporter(
                tracker,
                jobId,
                context.TotalRows,
                settings.ExportProgressBatchSize
            );

            var file = await handler.ExportAsync(
                filterJson,
                ExportFormats.Parse(format),
                progress,
                cancellationToken
            );

            await using var content = file.Content;
            if (content.CanSeek)
                content.Position = 0;

            var uploaded = await fileStorage.UploadRawAsync(
                content,
                file.FileName,
                file.ContentType,
                $"{settings.StorageFolder}/{module}",
                cancellationToken
            );

            await tracker.CompleteExportAsync(
                jobId,
                file.FileName,
                uploaded,
                progress.Processed,
                settings.FileRetentionHours,
                CancellationToken.None
            );
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "ExportJob thất bại JobId {JobId} Module {Module}", jobId, module);
            await tracker.FailAsync(
                jobId,
                ImportExportJobErrors.Describe(ex),
                CancellationToken.None
            );
            throw;
        }
    }
}
