namespace Cinema.Application.Interfaces;

public enum ImportFileFormat
{
    Xlsx,
    Csv,
}

public sealed record ImportRawRow(int RowNumber, IReadOnlyList<string?> Cells);

public sealed record ImportTable(IReadOnlyList<string> Headers, IReadOnlyList<ImportRawRow> Rows);

public interface IImportFileReader
{
    Task<ImportTable> ReadAsync(
        Stream content,
        ImportFileFormat format,
        string? preferredSheetName,
        int maxRows,
        CancellationToken cancellationToken
    );
}
