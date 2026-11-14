using Cinema.Application.DTOs.Common;
using Cinema.Application.DTOs.Import;

namespace Cinema.Application.Interfaces;

public interface IImportContext { }

public sealed record ImportColumn(string Name, bool Required, string Description, string Format);

public sealed record ImportRow<TRow>(
    int RowNumber,
    IReadOnlyList<string?> Cells,
    IReadOnlyDictionary<string, string?> Values,
    TRow Data,
    IReadOnlyList<ImportCellError> Errors
)
{
    public bool IsValid => Errors.Count == 0;
}

public sealed record ImportParseResult<TRow>(
    IReadOnlyList<string> Headers,
    IReadOnlyList<ImportRow<TRow>> Rows
);

public interface IImportProcessor<TRow>
{
    string Module { get; }
    string SheetName { get; }

    Task<Stream> BuildTemplateAsync(CancellationToken cancellationToken);

    Task<ImportParseResult<TRow>> ParseAsync(
        Stream content,
        ImportFileFormat format,
        CancellationToken cancellationToken
    );

    Task<IImportContext> CreateContextAsync(
        IReadOnlyList<ImportRow<TRow>> rows,
        CancellationToken cancellationToken
    );

    ImportRow<TRow> ValidateRow(ImportRow<TRow> row, IImportContext context);

    Task<BulkActionResponse> ApplyRowsAsync(
        IReadOnlyList<ImportRow<TRow>> rows,
        CancellationToken cancellationToken
    );
}
