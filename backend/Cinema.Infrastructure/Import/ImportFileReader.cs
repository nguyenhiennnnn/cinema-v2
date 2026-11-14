using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Cinema.Application.Interfaces;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;

namespace Cinema.Infrastructure.Import;

public partial class ImportFileReader : IImportFileReader
{
    private const string DateFormat = "dd/MM/yyyy";
    private const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";

    public async Task<ImportTable> ReadAsync(
        Stream content,
        ImportFileFormat format,
        string? preferredSheetName,
        int maxRows,
        CancellationToken cancellationToken
    )
    {
        var builder = new TableBuilder(maxRows);

        if (format == ImportFileFormat.Csv)
            await ReadCsvAsync(content, builder, cancellationToken);
        else
            ReadXlsx(content, builder, preferredSheetName, cancellationToken);

        return builder.Build();
    }

    private static void ReadXlsx(
        Stream content,
        TableBuilder builder,
        string? preferredSheetName,
        CancellationToken cancellationToken
    )
    {
        try
        {
            using var workbook = new XLWorkbook(content);

            var sheet =
                workbook.Worksheets.FirstOrDefault(s =>
                    string.Equals(s.Name, preferredSheetName, StringComparison.OrdinalIgnoreCase)
                ) ?? workbook.Worksheets.FirstOrDefault();
            if (sheet is null)
                return;

            foreach (var row in sheet.RowsUsed())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var cells = new string?[builder.ColumnLimit];
                for (var c = 0; c < cells.Length; c++)
                    cells[c] = CellText(row.Cell(c + 1));

                builder.Add(row.RowNumber(), cells);
            }
        }
        catch (Exception ex)
            when (ex is not (OperationCanceledException or AppException))
        {
            throw Invalid(Messages.Invalid_ImportFileContent);
        }
    }

    private static async Task ReadCsvAsync(
        Stream content,
        TableBuilder builder,
        CancellationToken cancellationToken
    )
    {
        var encoding = new UTF8Encoding(false, true);
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            DetectDelimiter = true,
            HasHeaderRecord = false,
            BadDataFound = null,
            MissingFieldFound = null,
        };

        try
        {
            using var reader = new StreamReader(content, encoding, true, leaveOpen: true);
            using var csv = new CsvReader(reader, configuration);

            while (await csv.ReadAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var count = Math.Min(csv.Parser.Count, TableBuilder.MaxColumns);
                var cells = new string?[count];
                for (var c = 0; c < count; c++)
                    cells[c] = csv.GetField(c);

                builder.Add(csv.Parser.Row, cells);
            }
        }
        catch (DecoderFallbackException)
        {
            throw Invalid(Messages.Invalid_ImportCsvEncoding);
        }
        catch (Exception ex)
            when (ex is CsvHelperException or IOException or InvalidOperationException)
        {
            throw Invalid(Messages.Invalid_ImportFileContent);
        }
    }

    private static string? CellText(IXLCell cell)
    {
        var value = cell.Value;

        if (value.IsBlank)
            return null;

        if (value.IsDateTime)
            return FormatDate(value.GetDateTime());

        if (value.IsNumber)
        {
            var number = value.GetNumber();
            return IsDateStyle(cell) && number > 0 && number < 2_958_466
                ? FormatDate(DateTime.FromOADate(number))
                : FormatNumber(number);
        }

        if (value.IsBoolean)
            return value.GetBoolean() ? "TRUE" : "FALSE";

        if (value.IsText)
            return value.GetText();

        return null;
    }

    private static string FormatDate(DateTime value) =>
        value.TimeOfDay == TimeSpan.Zero
            ? value.ToString(DateFormat, CultureInfo.InvariantCulture)
            : value.ToString(DateTimeFormat, CultureInfo.InvariantCulture);

    private static string FormatNumber(double number) =>
        number == Math.Floor(number) && Math.Abs(number) < 1e15
            ? ((long)number).ToString(CultureInfo.InvariantCulture)
            : number.ToString("R", CultureInfo.InvariantCulture);

    private static bool IsDateStyle(IXLCell cell)
    {
        var format = cell.Style.NumberFormat;
        var id = format.NumberFormatId;
        if (id is >= 14 and <= 22 or >= 45 and <= 47)
            return true;

        var code = format.Format;
        if (string.IsNullOrEmpty(code))
            return false;

        var stripped = NonDateSegments().Replace(code, string.Empty);
        return DateTokens().IsMatch(stripped);
    }

    [GeneratedRegex("\"[^\"]*\"|\\[[^\\]]*\\]|\\\\.|_.|\\*.")]
    private static partial Regex NonDateSegments();

    [GeneratedRegex("[dmyhs]", RegexOptions.IgnoreCase)]
    private static partial Regex DateTokens();

    private static ValidationAppException Invalid(string message) =>
        new(message, new Dictionary<string, string[]> { ["file"] = [message] });

    private sealed class TableBuilder(int maxRows)
    {
        public const int MaxColumns = 100;

        private readonly List<ImportRawRow> rows = [];
        private List<string>? headers;

        public int ColumnLimit => headers?.Count ?? MaxColumns;

        public void Add(int rowNumber, IReadOnlyList<string?> cells)
        {
            if (headers is null)
            {
                var last = -1;
                for (var i = 0; i < cells.Count && i < MaxColumns; i++)
                {
                    if (!string.IsNullOrWhiteSpace(cells[i]))
                        last = i;
                }

                if (last < 0)
                    return;

                headers = cells.Take(last + 1).Select(c => c?.Trim() ?? string.Empty).ToList();
                return;
            }

            var values = new string?[headers.Count];
            var hasValue = false;
            for (var i = 0; i < headers.Count && i < cells.Count; i++)
            {
                values[i] = cells[i];
                if (!string.IsNullOrWhiteSpace(cells[i]))
                    hasValue = true;
            }

            if (!hasValue)
                return;

            if (rows.Count >= maxRows)
                throw Invalid(string.Format(Messages.Invalid_ImportTooManyRows, maxRows));

            rows.Add(new ImportRawRow(rowNumber, values));
        }

        public ImportTable Build() => new(headers ?? [], rows);
    }
}
