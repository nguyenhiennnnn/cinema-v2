using Cinema.Application.DTOs.ImportExport;
using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.ImportExport;

public class ImportExportJobTracker(
    IServiceScopeFactory scopeFactory,
    ILogger<ImportExportJobTracker> logger
) : IImportExportJobTracker
{
    private const int MaxFileNameLength = 255;
    private const int MaxErrorLength = 2000;

    public async Task<Guid> CreateQueuedAsync(
        ImportExportJobType type,
        string module,
        string format,
        int userId,
        string? fileName,
        int totalRows,
        CancellationToken cancellationToken
    )
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var job = new ImportExportJob
        {
            JobId = Guid.NewGuid(),
            Type = type,
            Module = module,
            Format = format,
            Status = ImportExportJobStatus.Queued,
            Progress = 0,
            TotalRows = totalRows,
            FileName = Truncate(fileName, MaxFileNameLength),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
        };

        db.ImportExportJobs.Add(job);
        await db.SaveChangesAsync(cancellationToken);
        return job.JobId;
    }

    public async Task<ImportExportJobRunContext?> MarkRunningAsync(
        Guid jobId,
        CancellationToken cancellationToken
    )
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var job = await FindAsync(db, jobId, cancellationToken);
        if (job is null)
            return null;

        job.Status = ImportExportJobStatus.Running;
        job.Progress = 0;
        job.StartedAt = DateTime.UtcNow;
        job.CompletedAt = null;
        job.ErrorMessage = null;
        await db.SaveChangesAsync(cancellationToken);

        await NotifyAsync(
            scope,
            job,
            (notifier, dto) =>
                notifier.NotifyImportExportJobProgressAsync(
                    job.CreatedByUserId,
                    dto,
                    cancellationToken
                )
        );

        return new ImportExportJobRunContext(job.CreatedByUserId, job.TotalRows);
    }

    public async Task ReportProgressAsync(
        Guid jobId,
        int progress,
        CancellationToken cancellationToken
    )
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var job = await FindAsync(db, jobId, cancellationToken);
        if (job is null || job.Status != ImportExportJobStatus.Running)
            return;

        job.Progress = Math.Clamp(progress, 0, 100);
        await db.SaveChangesAsync(cancellationToken);

        await NotifyAsync(
            scope,
            job,
            (notifier, dto) =>
                notifier.NotifyImportExportJobProgressAsync(
                    job.CreatedByUserId,
                    dto,
                    cancellationToken
                )
        );
    }

    public async Task CompleteExportAsync(
        Guid jobId,
        string fileName,
        FileUploadResult file,
        int totalRows,
        int retentionHours,
        CancellationToken cancellationToken
    )
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var job = await FindAsync(db, jobId, cancellationToken);
        if (job is null)
            return;

        var now = DateTime.UtcNow;
        job.Status = ImportExportJobStatus.Completed;
        job.Progress = 100;
        job.TotalRows = totalRows;
        job.SuccessRows = totalRows;
        job.FailedRows = 0;
        job.FileName = Truncate(fileName, MaxFileNameLength);
        job.StoragePath = file.PublicId;
        job.FileUrl = file.Url;
        job.ErrorMessage = null;
        job.CompletedAt = now;
        job.ExpiresAt = now.AddHours(retentionHours);
        await db.SaveChangesAsync(cancellationToken);

        await NotifyAsync(
            scope,
            job,
            (notifier, dto) =>
                notifier.NotifyImportExportJobCompletedAsync(
                    job.CreatedByUserId,
                    dto,
                    cancellationToken
                )
        );
    }

    public async Task CompleteImportAsync(
        Guid jobId,
        int successRows,
        int failedRows,
        string? errorMessage,
        CancellationToken cancellationToken
    )
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var job = await FindAsync(db, jobId, cancellationToken);
        if (job is null)
            return;

        var failed = ResolveImportStatus(successRows, failedRows) == ImportExportJobStatus.Failed;
        job.Status = failed ? ImportExportJobStatus.Failed : ImportExportJobStatus.Completed;
        job.Progress = 100;
        job.TotalRows = successRows + failedRows;
        job.SuccessRows = successRows;
        job.FailedRows = failedRows;
        job.ErrorMessage = failed ? Truncate(errorMessage, MaxErrorLength) : null;
        job.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        await NotifyAsync(
            scope,
            job,
            (notifier, dto) =>
                failed
                    ? notifier.NotifyImportExportJobFailedAsync(
                        job.CreatedByUserId,
                        dto,
                        cancellationToken
                    )
                    : notifier.NotifyImportExportJobCompletedAsync(
                        job.CreatedByUserId,
                        dto,
                        cancellationToken
                    )
        );
    }

    public async Task FailAsync(
        Guid jobId,
        string errorMessage,
        CancellationToken cancellationToken
    )
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var job = await FindAsync(db, jobId, cancellationToken);
        if (job is null)
            return;

        job.Status = ImportExportJobStatus.Failed;
        job.ErrorMessage = Truncate(errorMessage, MaxErrorLength);
        job.CompletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        await NotifyAsync(
            scope,
            job,
            (notifier, dto) =>
                notifier.NotifyImportExportJobFailedAsync(
                    job.CreatedByUserId,
                    dto,
                    cancellationToken
                )
        );
    }

    public async Task RecordCompletedAsync(
        ImportExportJobType type,
        string module,
        string format,
        int userId,
        string? fileName,
        int totalRows,
        int successRows,
        int failedRows,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var now = DateTime.UtcNow;
            var status =
                type == ImportExportJobType.Import
                    ? ResolveImportStatus(successRows, failedRows)
                    : ImportExportJobStatus.Completed;

            db.ImportExportJobs.Add(
                new ImportExportJob
                {
                    JobId = Guid.NewGuid(),
                    Type = type,
                    Module = module,
                    Format = format,
                    Status = status,
                    Progress = 100,
                    TotalRows = totalRows,
                    SuccessRows = successRows,
                    FailedRows = failedRows,
                    FileName = Truncate(fileName, MaxFileNameLength),
                    CreatedByUserId = userId,
                    CreatedAt = now,
                    StartedAt = now,
                    CompletedAt = now,
                }
            );
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(
                ex,
                "Không ghi được lịch sử {Type} {Module} của UserId {UserId}",
                type,
                module,
                userId
            );
        }
    }

    private static ImportExportJobStatus ResolveImportStatus(int successRows, int failedRows) =>
        successRows == 0 && failedRows > 0
            ? ImportExportJobStatus.Failed
            : ImportExportJobStatus.Completed;

    private static Task<ImportExportJob?> FindAsync(
        IAppDbContext db,
        Guid jobId,
        CancellationToken cancellationToken
    ) => db.ImportExportJobs.FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);

    private async Task NotifyAsync(
        AsyncServiceScope scope,
        ImportExportJob job,
        Func<IRealtimeNotifier, ImportExportJobDto, Task> send
    )
    {
        try
        {
            var notifier = scope.ServiceProvider.GetRequiredService<IRealtimeNotifier>();
            await send(notifier, job.ToDto(DateTime.UtcNow));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Không đẩy được realtime cho JobId {JobId}", job.JobId);
        }
    }

    private static string? Truncate(string? value, int maxLength) =>
        value is null || value.Length <= maxLength ? value : value[..maxLength];
}
