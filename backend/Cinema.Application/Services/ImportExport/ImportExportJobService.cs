using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.ImportExport;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.ImportExport;

public class ImportExportJobService(
    IAppDbContext db,
    IFileStorage fileStorage,
    IOptions<ImportExportOptions> options,
    ILogger<ImportExportJobService> logger
) : IImportExportJobService
{
    private const string RawResourceType = "raw";
    private const int MaxPageSize = 100;

    public async Task<OffsetPageResult<ImportExportJobDto>> GetJobsAsync(
        int userId,
        ImportExportJobListFilter filter,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    )
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = db.ImportExportJobs.AsNoTracking().AsQueryable();
        if (!CanViewAll(userId))
            query = query.Where(j => j.CreatedByUserId == userId);

        if (!string.IsNullOrWhiteSpace(filter.Type))
        {
            var type = ParseEnum<ImportExportJobType>(filter.Type, "type");
            query = query.Where(j => j.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var status = ParseEnum<ImportExportJobStatus>(filter.Status, "status");
            query = query.Where(j => j.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(filter.Module))
        {
            var module = filter.Module.Trim().ToLowerInvariant();
            query = query.Where(j => j.Module == module);
        }

        var total = await query.CountAsync(cancellationToken);
        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .ThenByDescending(j => j.JobId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        return OffsetPageResult<ImportExportJobDto>.Create(
            jobs.Select(j => j.ToDto(now)).ToList(),
            page,
            pageSize,
            total
        );
    }

    public async Task<ImportExportJobDto> GetJobAsync(
        int userId,
        Guid jobId,
        CancellationToken cancellationToken
    )
    {
        var job = await FindAccessibleAsync(userId, jobId, cancellationToken);
        return job.ToDto(DateTime.UtcNow);
    }

    public async Task<ExportFileDto> DownloadAsync(
        int userId,
        Guid jobId,
        CancellationToken cancellationToken
    )
    {
        var job = await FindAccessibleAsync(userId, jobId, cancellationToken);

        if (job.Type != ImportExportJobType.Export)
            throw new BusinessRuleException(Messages.Rule_ImportExportJobNoFile);

        if (job.Status != ImportExportJobStatus.Completed)
            throw new BusinessRuleException(Messages.Rule_ImportExportJobNotReady);

        if (job.ExpiresAt is null && string.IsNullOrEmpty(job.FileUrl))
            throw new BusinessRuleException(Messages.Rule_ImportExportJobNoFile);

        if (
            job.ExpiresAt is { } expiresAt && expiresAt <= DateTime.UtcNow
            || string.IsNullOrEmpty(job.FileUrl)
        )
            throw new BusinessRuleException(Messages.Rule_ImportExportFileExpired);

        Stream content;
        try
        {
            content = await fileStorage.OpenReadAsync(job.FileUrl, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException and not AppException)
        {
            logger.LogWarning(ex, "Tải file thất bại JobId {JobId}", job.JobId);
            throw new BusinessRuleException(Messages.Rule_ImportExportFileDownloadFailed);
        }

        return new ExportFileDto(
            content,
            ExportFormats.ContentType(ExportFormats.Parse(job.Format)),
            job.FileName ?? $"{job.Module}.{job.Format}"
        );
    }

    public async Task<int> CleanupExpiredFilesAsync(CancellationToken cancellationToken)
    {
        var batchSize = Math.Max(1, options.Value.CleanupBatchSize);
        var cleaned = 0;

        while (true)
        {
            var now = DateTime.UtcNow;
            var jobs = await db
                .ImportExportJobs.Where(j =>
                    j.StoragePath != null && j.ExpiresAt != null && j.ExpiresAt <= now
                )
                .OrderBy(j => j.ExpiresAt)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            if (jobs.Count == 0)
                break;

            var cleanedInBatch = 0;
            foreach (var job in jobs)
            {
                try
                {
                    await fileStorage.DeleteAsync(job.StoragePath!, RawResourceType, cancellationToken);
                    job.StoragePath = null;
                    job.FileUrl = null;
                    cleanedInBatch++;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogWarning(ex, "Xoá file hết hạn thất bại JobId {JobId}", job.JobId);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            cleaned += cleanedInBatch;

            if (cleanedInBatch == 0 || jobs.Count < batchSize)
                break;
        }

        if (cleaned > 0)
            logger.LogInformation("ImportExportCleanup: đã xoá {Count} file hết hạn.", cleaned);

        return cleaned;
    }

    private bool CanViewAll(int userId) => options.Value.SuperAdminUserIds.Contains(userId);

    private async Task<ImportExportJob> FindAccessibleAsync(
        int userId,
        Guid jobId,
        CancellationToken cancellationToken
    )
    {
        var job = await db
            .ImportExportJobs.AsNoTracking()
            .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);

        if (job is null || (job.CreatedByUserId != userId && !CanViewAll(userId)))
            throw new NotFoundException(Messages.NotFound_ImportExportJob);

        return job;
    }

    private static TEnum ParseEnum<TEnum>(string value, string field)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value.Trim(), ignoreCase: true, out var parsed))
            return parsed;

        throw new ValidationAppException(
            Messages.Invalid_ImportExportJobFilter,
            new Dictionary<string, string[]> { [field] = [Messages.Invalid_ImportExportJobFilter] }
        );
    }
}
