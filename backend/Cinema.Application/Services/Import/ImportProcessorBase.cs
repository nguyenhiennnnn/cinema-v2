using Cinema.Application.DTOs.Common;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;

namespace Cinema.Application.Services.Import;

public abstract class ImportProcessorBase<TRow>(IImportFileReader fileReader)
    : IImportProcessor<TRow>
{
    public abstract string Module { get; }
    public abstract string SheetName { get; }

    protected abstract IReadOnlyList<ImportColumn> Columns { get; }

    protected abstract TRow MapRow(IReadOnlyDictionary<string, string?> values);

    public abstract Task<Stream> BuildTemplateAsync(CancellationToken cancellationToken);

    public abstract Task<IImportContext> CreateContextAsync(
        IReadOnlyList<ImportRow<TRow>> rows,
        CancellationToken cancellationToken
    );

    public abstract ImportRow<TRow> ValidateRow(ImportRow<TRow> row, IImportContext context);

    public abstract Task<BulkActionResponse> ApplyRowsAsync(
        IReadOnlyList<ImportRow<TRow>> rows,
        CancellationToken cancellationToken
    );

    public async Task<ImportParseResult<TRow>> ParseAsync(
        Stream content,
        ImportFileFormat format,
        CancellationToken cancellationToken
    )
    {
        var table = await fileReader.ReadAsync(
            content,
            format,
            SheetName,
            ImportLimits.MaxRows,
            cancellationToken
        );

        var indexes = MapHeaders(table.Headers);
        var rows = new List<ImportRow<TRow>>(table.Rows.Count);

        foreach (var raw in table.Rows)
        {
            var values = new Dictionary<string, string?>(Columns.Count);
            foreach (var column in Columns)
            {
                string? value = null;
                if (indexes.TryGetValue(column.Name, out var index) && index < raw.Cells.Count)
                    value = Clean(raw.Cells[index]);

                values[column.Name] = value;
            }

            rows.Add(new ImportRow<TRow>(raw.RowNumber, raw.Cells, values, MapRow(values), []));
        }

        return new ImportParseResult<TRow>(table.Headers, rows);
    }

    private Dictionary<string, int> MapHeaders(IReadOnlyList<string> headers)
    {
        var columnsByKey = Columns.ToDictionary(
            c => ImportHeaderNormalizer.Normalize(c.Name),
            c => c.Name
        );

        var indexes = new Dictionary<string, int>();
        var duplicated = new List<string>();

        for (var i = 0; i < headers.Count; i++)
        {
            if (!columnsByKey.TryGetValue(ImportHeaderNormalizer.Normalize(headers[i]), out var name))
                continue;

            if (!indexes.TryAdd(name, i))
                duplicated.Add(name);
        }

        if (duplicated.Count > 0)
            throw Invalid(
                string.Format(
                    Messages.Invalid_ImportDuplicateColumns,
                    string.Join(", ", duplicated.Distinct())
                )
            );

        var missing = Columns
            .Where(c => c.Required && !indexes.ContainsKey(c.Name))
            .Select(c => c.Name)
            .ToList();
        if (missing.Count > 0)
            throw Invalid(
                string.Format(Messages.Invalid_ImportMissingColumns, string.Join(", ", missing))
            );

        return indexes;
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ValidationAppException Invalid(string message) =>
        new(message, new Dictionary<string, string[]> { ["file"] = [message] });
}
