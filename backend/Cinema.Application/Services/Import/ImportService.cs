using Cinema.Application.DTOs.Common;
using Cinema.Application.BackgroundJobs;
using Cinema.Application.DTOs.Import;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.ImportExport;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Import;

public class ImportService<TRow>(
    IImportProcessor<TRow> processor,
    IImportSessionStore sessions,
    IExportWriter exportWriter,
    IImportExportJobTracker jobTracker,
    IBackgroundJobClient backgroundJobClient,
    IOptions<ImportExportOptions> options
) : IImportService<TRow>
{
    public async Task<ExportFileDto> GetTemplateAsync(CancellationToken cancellationToken)
    {
        var content = await processor.BuildTemplateAsync(cancellationToken);
        return new ExportFileDto(
            content,
            ExportFormats.ContentType(ExportFormat.Xlsx),
            ExportFileName.Create(
                $"{processor.Module}-import-template",
                ExportFormats.Extension(ExportFormat.Xlsx)
            )
        );
    }

    public async Task<ImportPreviewResponse> PreviewAsync(
        int userId,
        string? fileName,
        Stream content,
        CancellationToken cancellationToken
    )
    {
        using var upload = await ImportFileGuard.ReadAsync(fileName, content, cancellationToken);

        var parsed = await processor.ParseAsync(upload.Content, upload.Format, cancellationToken);
        if (parsed.Rows.Count == 0)
            throw new ValidationAppException(
                Messages.Invalid_ImportNoDataRows,
                new Dictionary<string, string[]> { ["file"] = [Messages.Invalid_ImportNoDataRows] }
            );

        var context = await processor.CreateContextAsync(parsed.Rows, cancellationToken);
        var validated = parsed.Rows.Select(row => processor.ValidateRow(row, context)).ToList();

        var session = sessions.Create(
            userId,
            fileName,
            upload.Format,
            parsed.Headers,
            validated
        );
        var validCount = validated.Count(row => row.IsValid);

        return new ImportPreviewResponse(
            session.PreviewId,
            validated.Count,
            validCount,
            validated.Count - validCount,
            validated
                .Select(row => new ImportPreviewRow(
                    row.RowNumber,
                    row.Values,
                    row.Errors.ToList()
                ))
                .ToList()
        );
    }

    public async Task<ImportApplyOutcome> ApplyAsync(
        int userId,
        ImportApplyRequest request,
        CancellationToken cancellationToken
    )
    {
        var session = sessions.Get<TRow>(request.PreviewId, userId);
        var plan = PlanApply(session, request.SkipInvalidRows);

        if (!session.TryBeginApply())
            throw new ConflictAppException(Messages.Conflict_ImportApplyInProgress);

        if (plan.ValidRows.Count > options.Value.ImportBackgroundThreshold)
        {
            try
            {
                var jobId = await EnqueueApplyAsync(
                    userId,
                    session,
                    request,
                    plan.ValidRows.Count,
                    cancellationToken
                );
                return new ImportApplyOutcome(null, jobId);
            }
            catch
            {
                session.EndApply();
                throw;
            }
        }

        try
        {
            var response = await ExecuteApplyAsync(session, plan, null, cancellationToken);

            await jobTracker.RecordCompletedAsync(
                ImportExportJobType.Import,
                processor.Module,
                FormatName(session),
                userId,
                session.FileName,
                session.Rows.Count,
                response.TotalSuccess,
                response.TotalFailed,
                cancellationToken
            );

            return new ImportApplyOutcome(response, null);
        }
        finally
        {
            session.EndApply();
        }
    }

    public async Task<BulkActionResponse> RunQueuedApplyAsync(
        int userId,
        string previewId,
        bool skipInvalidRows,
        IJobProgressReporter progress,
        CancellationToken cancellationToken
    )
    {
        var session = sessions.Get<TRow>(previewId, userId);

        try
        {
            var plan = PlanApply(session, skipInvalidRows);
            return await ExecuteApplyAsync(session, plan, progress, cancellationToken);
        }
        finally
        {
            session.EndApply();
        }
    }

    private async Task<Guid> EnqueueApplyAsync(
        int userId,
        ImportSession<TRow> session,
        ImportApplyRequest request,
        int validRowCount,
        CancellationToken cancellationToken
    )
    {
        var module = processor.Module;
        var previewId = request.PreviewId;
        var skipInvalidRows = request.SkipInvalidRows;

        var jobId = await jobTracker.CreateQueuedAsync(
            ImportExportJobType.Import,
            module,
            FormatName(session),
            userId,
            session.FileName,
            validRowCount,
            cancellationToken
        );

        try
        {
            backgroundJobClient.Enqueue<ImportApplyJob>(j =>
                j.RunAsync(jobId, module, previewId, skipInvalidRows, CancellationToken.None)
            );
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await jobTracker.FailAsync(
                jobId,
                ImportExportJobErrors.Describe(ex),
                CancellationToken.None
            );
            throw;
        }

        return jobId;
    }

    private async Task<BulkActionResponse> ExecuteApplyAsync(
        ImportSession<TRow> session,
        ApplyPlan plan,
        IJobProgressReporter? progress,
        CancellationToken cancellationToken
    )
    {
        var applied = await processor.ApplyRowsAsync(plan.ValidRows, progress, cancellationToken);
        if (applied.TotalFailed == 0)
            sessions.Remove(session.PreviewId);

        var skipped = plan.InvalidRows.Select(row => new BulkActionResultItem(
            0,
            false,
            FormatErrors(row.Errors),
            row.RowNumber
        ));

        var results = applied.Results.Concat(skipped).OrderBy(item => item.RowNumber).ToList();
        return BulkOperation.Build(results);
    }

    private static ApplyPlan PlanApply(ImportSession<TRow> session, bool skipInvalidRows)
    {
        var invalidRows = session.Rows.Where(row => !row.IsValid).ToList();
        if (invalidRows.Count > 0 && !skipInvalidRows)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_ImportHasInvalidRows, invalidRows.Count)
            );

        var validRows = session.Rows.Where(row => row.IsValid).ToList();
        if (validRows.Count == 0)
            throw new BusinessRuleException(Messages.Rule_ImportNoValidRows);

        return new ApplyPlan(validRows, invalidRows);
    }

    private static string FormatName(ImportSession<TRow> session) =>
        session.Format.ToString().ToLowerInvariant();

    private sealed record ApplyPlan(
        List<ImportRow<TRow>> ValidRows,
        List<ImportRow<TRow>> InvalidRows
    );

    public async Task<ExportFileDto> GetErrorFileAsync(
        int userId,
        string previewId,
        CancellationToken cancellationToken
    )
    {
        var session = sessions.Get<TRow>(previewId, userId);
        if (session.Rows.All(row => row.IsValid))
            throw new BusinessRuleException(Messages.Rule_ImportNoErrors);

        var columns = new List<ExportColumn<ImportRow<TRow>>>(session.Headers.Count + 1);
        for (var i = 0; i < session.Headers.Count; i++)
        {
            var index = i;
            columns.Add(
                new ExportColumn<ImportRow<TRow>>(
                    session.Headers[index],
                    row => index < row.Cells.Count ? row.Cells[index] : null
                )
            );
        }

        columns.Add(
            new ExportColumn<ImportRow<TRow>>(
                Messages.Import_ColErrorReason,
                row => FormatErrors(row.Errors)
            )
        );

        var content = await exportWriter.WriteXlsxAsync(
            processor.SheetName,
            columns,
            session.Rows,
            null,
            cancellationToken
        );

        return new ExportFileDto(
            content,
            ExportFormats.ContentType(ExportFormat.Xlsx),
            ExportFileName.Create(
                $"{processor.Module}-import-errors",
                ExportFormats.Extension(ExportFormat.Xlsx)
            )
        );
    }

    private static string? FormatErrors(IReadOnlyList<ImportCellError> errors) =>
        errors.Count == 0
            ? null
            : string.Join("; ", errors.Select(error => $"{error.Column}: {error.Message}"));
}
