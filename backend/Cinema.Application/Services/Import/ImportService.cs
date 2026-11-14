using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Import;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;

namespace Cinema.Application.Services.Import;

public class ImportService<TRow>(
    IImportProcessor<TRow> processor,
    IImportSessionStore sessions,
    IExportWriter exportWriter
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

        var session = sessions.Create(userId, parsed.Headers, validated);
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

    public async Task<BulkActionResponse> ApplyAsync(
        int userId,
        ImportApplyRequest request,
        CancellationToken cancellationToken
    )
    {
        var session = sessions.Get<TRow>(request.PreviewId, userId);

        var invalidRows = session.Rows.Where(row => !row.IsValid).ToList();
        if (invalidRows.Count > 0 && !request.SkipInvalidRows)
            throw new BusinessRuleException(
                string.Format(Messages.Rule_ImportHasInvalidRows, invalidRows.Count)
            );

        var validRows = session.Rows.Where(row => row.IsValid).ToList();
        if (validRows.Count == 0)
            throw new BusinessRuleException(Messages.Rule_ImportNoValidRows);

        if (!session.TryBeginApply())
            throw new ConflictAppException(Messages.Conflict_ImportApplyInProgress);

        try
        {
            var applied = await processor.ApplyRowsAsync(validRows, cancellationToken);
            if (applied.TotalFailed == 0)
                sessions.Remove(session.PreviewId);

            var skipped = invalidRows.Select(row => new BulkActionResultItem(
                0,
                false,
                FormatErrors(row.Errors),
                row.RowNumber
            ));

            var results = applied.Results.Concat(skipped).OrderBy(item => item.RowNumber).ToList();
            return BulkOperation.Build(results);
        }
        finally
        {
            session.EndApply();
        }
    }

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
