using System.Globalization;
using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.ImportExport;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.ImportExport;

public class ExportJobService(
    IEnumerable<IExportModule> modules,
    IImportExportJobTracker tracker,
    IBackgroundJobClient backgroundJobClient,
    IOptions<ImportExportOptions> options
) : IExportJobService
{
    public async Task<ExportFileDto> ExportNowAsync<TFilter>(
        int userId,
        string module,
        TFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var handler = Resolve(module);
        var filterJson = ImportExportJson.Serialize(filter);
        var threshold = options.Value.ExportBackgroundThreshold;

        var estimated = await handler.CountAsync(filterJson, cancellationToken);
        if (estimated > threshold)
            throw new BusinessRuleException(
                string.Format(
                    Messages.Rule_ExportUseBackgroundJob,
                    threshold.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))
                )
            );

        var counter = new RowCountReporter();
        var file = await handler.ExportAsync(filterJson, format, counter, cancellationToken);

        await tracker.RecordCompletedAsync(
            ImportExportJobType.Export,
            module,
            ExportFormats.Extension(format),
            userId,
            file.FileName,
            counter.Count,
            counter.Count,
            0,
            cancellationToken
        );

        return file;
    }

    public async Task<ImportExportJobAcceptedDto> EnqueueAsync<TFilter>(
        int userId,
        string module,
        TFilter filter,
        ExportFormat format,
        CancellationToken cancellationToken
    )
    {
        var handler = Resolve(module);
        var filterJson = ImportExportJson.Serialize(filter);

        var estimated = await handler.CountAsync(filterJson, cancellationToken);
        ExportLimits.EnsureWithinLimit(estimated);

        var formatName = ExportFormats.Extension(format);
        var jobId = await tracker.CreateQueuedAsync(
            ImportExportJobType.Export,
            module,
            formatName,
            userId,
            null,
            estimated,
            cancellationToken
        );

        try
        {
            backgroundJobClient.Enqueue<ExportJob>(j =>
                j.RunAsync(jobId, module, filterJson, formatName, CancellationToken.None)
            );
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await tracker.FailAsync(
                jobId,
                ImportExportJobErrors.Describe(ex),
                CancellationToken.None
            );
            throw;
        }

        return new ImportExportJobAcceptedDto(jobId);
    }

    private IExportModule Resolve(string module) =>
        modules.FirstOrDefault(m => m.Module == module)
        ?? throw new InvalidOperationException($"Module xuất dữ liệu '{module}' chưa được đăng ký.");
}
