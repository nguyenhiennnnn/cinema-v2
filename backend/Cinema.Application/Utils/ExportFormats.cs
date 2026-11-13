using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;

namespace Cinema.Application.Utils;

public enum ExportFormat
{
    Xlsx,
    Csv,
}

public static class ExportFormats
{
    private const string XlsxContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string CsvContentType = "text/csv; charset=utf-8";

    public static ExportFormat Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ExportFormat.Xlsx;

        return value.Trim().ToLowerInvariant() switch
        {
            "xlsx" => ExportFormat.Xlsx,
            "csv" => ExportFormat.Csv,
            _ => throw new ValidationAppException(
                Messages.Export_InvalidFormat,
                new Dictionary<string, string[]> { ["format"] = [Messages.Export_InvalidFormat] }
            ),
        };
    }

    public static string ContentType(ExportFormat format) =>
        format == ExportFormat.Csv ? CsvContentType : XlsxContentType;

    public static string Extension(ExportFormat format) =>
        format == ExportFormat.Csv ? "csv" : "xlsx";
}
