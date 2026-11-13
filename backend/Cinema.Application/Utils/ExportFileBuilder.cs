using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;

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
        CancellationToken cancellationToken
    )
    {
        var content =
            format == ExportFormat.Csv
                ? await writer.WriteCsvAsync(columns, rows, cancellationToken)
                : await writer.WriteXlsxAsync(sheetName, columns, rows, null, cancellationToken);

        return new ExportFileDto(
            content,
            ExportFormats.ContentType(format),
            ExportFileName.Create(module, ExportFormats.Extension(format))
        );
    }
}
