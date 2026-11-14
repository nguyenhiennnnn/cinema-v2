using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;

namespace Cinema.Application.Utils;

public enum ExportFormat
{
    Xlsx,
    Csv,
    Pdf,
}

public static class ExportFormats
{
    private const string XlsxContentType =
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string CsvContentType = "text/csv; charset=utf-8";
    private const string PdfContentType = "application/pdf";

    public static ExportFormat Parse(string? value, bool allowPdf = false)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ExportFormat.Xlsx;

        var normalized = value.Trim().ToLowerInvariant();
        if (allowPdf && normalized == "pdf")
            return ExportFormat.Pdf;

        return normalized switch
        {
            "xlsx" => ExportFormat.Xlsx,
            "csv" => ExportFormat.Csv,
            _ => throw InvalidFormat(allowPdf),
        };
    }

    public static string ContentType(ExportFormat format) =>
        format switch
        {
            ExportFormat.Csv => CsvContentType,
            ExportFormat.Pdf => PdfContentType,
            _ => XlsxContentType,
        };

    public static string Extension(ExportFormat format) =>
        format switch
        {
            ExportFormat.Csv => "csv",
            ExportFormat.Pdf => "pdf",
            _ => "xlsx",
        };

    private static ValidationAppException InvalidFormat(bool allowPdf)
    {
        var message = allowPdf ? Messages.Export_InvalidReportFormat : Messages.Export_InvalidFormat;
        return new ValidationAppException(
            message,
            new Dictionary<string, string[]> { ["format"] = [message] }
        );
    }
}
