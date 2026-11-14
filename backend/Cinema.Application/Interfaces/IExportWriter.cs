namespace Cinema.Application.Interfaces;

public enum ExportColumnType
{
    Text,
    Number,
    Money,
    Date,
    DateTime,
}

public sealed record ExportColumn<T>(
    string Header,
    Func<T, object?> Selector,
    ExportColumnType Type = ExportColumnType.Text
);

public sealed record ExportSheet(
    string Name,
    IReadOnlyList<string> Headers,
    IReadOnlyList<IReadOnlyList<object?>> Rows
);

public interface IExportWriter
{
    Task<Stream> WriteXlsxAsync<T>(
        string sheetName,
        IReadOnlyList<ExportColumn<T>> columns,
        IEnumerable<T> rows,
        IReadOnlyList<object?>? totalRow = null,
        CancellationToken cancellationToken = default
    );

    Task<Stream> WriteXlsxAsync<T>(
        string sheetName,
        IReadOnlyList<ExportColumn<T>> columns,
        IAsyncEnumerable<T> rows,
        IReadOnlyList<object?>? totalRow = null,
        CancellationToken cancellationToken = default
    );

    Task<Stream> WriteXlsxWorkbookAsync(
        IReadOnlyList<ExportSheet> sheets,
        CancellationToken cancellationToken = default
    );

    Task<Stream> WriteCsvAsync<T>(
        IReadOnlyList<ExportColumn<T>> columns,
        IEnumerable<T> rows,
        CancellationToken cancellationToken = default
    );

    Task<Stream> WriteCsvAsync<T>(
        IReadOnlyList<ExportColumn<T>> columns,
        IAsyncEnumerable<T> rows,
        CancellationToken cancellationToken = default
    );
}
