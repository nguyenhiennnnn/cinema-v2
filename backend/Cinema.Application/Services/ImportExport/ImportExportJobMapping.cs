using Cinema.Application.DTOs.ImportExport;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;

namespace Cinema.Application.Services.ImportExport;

public static class ImportExportJobMapping
{
    public static ImportExportJobDto ToDto(this ImportExportJob job, DateTime utcNow)
    {
        var isExpired = job.ExpiresAt is { } expiresAt && expiresAt <= utcNow;
        var canDownload =
            job.Type == ImportExportJobType.Export
            && job.Status == ImportExportJobStatus.Completed
            && !string.IsNullOrEmpty(job.FileUrl)
            && !isExpired;

        return new ImportExportJobDto(
            job.JobId,
            job.Type.ToString(),
            job.Module,
            job.Format,
            job.Status.ToString(),
            job.Progress,
            job.TotalRows,
            job.SuccessRows,
            job.FailedRows,
            job.FileName,
            job.ErrorMessage,
            job.CreatedByUserId,
            job.CreatedAt,
            job.StartedAt,
            job.CompletedAt,
            job.ExpiresAt,
            isExpired,
            canDownload
        );
    }
}
