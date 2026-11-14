using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using System.Runtime.CompilerServices;

namespace Cinema.Application.Utils;

public static class ExportFileBuilder
{
    public static async Task<ExportFileDto> BuildAsync<T>(
        IExportWriter writer,
        string module,
        string sheetName,
        ExportFormat format,
        IReadOnlyList<ExportColumn<T>> columns,
        IAsyncEnumerable<T> rows,
        CancellationToken cancellationToken,
        IJobProgressReporter? progress = null
    )
    {
        var source = progress is null ? rows : TrackAsync(rows, progress, cancellationToken);
        var content =
            format == ExportFormat.Csv
                ? await writer.WriteCsvAsync(columns, source, cancellationToken)
                : await writer.WriteXlsxAsync(sheetName, columns, source, null, cancellationToken);

        return new ExportFileDto(
            content,
            ExportFormats.ContentType(format),
            ExportFileName.Create(module, ExportFormats.Extension(format))
        );
    }

    private static async IAsyncEnumerable<T> TrackAsync<T>(
        IAsyncEnumerable<T> rows,
        IJobProgressReporter progress,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        var processed = 0;
        await foreach (var row in rows.WithCancellation(cancellationToken))
        {
            yield return row;
            processed++;
            await progress.ReportAsync(processed, cancellationToken);
        }
    }
}
