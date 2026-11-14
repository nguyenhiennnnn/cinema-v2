using System.Globalization;
using Cinema.Application.Interfaces;

namespace Cinema.Application.Utils;

public static class PdfCellFormatter
{
    private const string NumberFormat = "#,##0.##";
    private const string DateFormat = "dd/MM/yyyy";
    private const string DateTimeFormat = "dd/MM/yyyy HH:mm";

    private static readonly CultureInfo ViCulture = CultureInfo.GetCultureInfo("vi-VN");

    public static string Format(object? value, ExportColumnType type)
    {
        if (value is null)
            return string.Empty;

        return type switch
        {
            ExportColumnType.Number or ExportColumnType.Money => FormatNumber(value),
            ExportColumnType.Date => FormatDate(value, DateFormat),
            ExportColumnType.DateTime => FormatDate(value, DateTimeFormat),
            _ => Convert.ToString(value, ViCulture) ?? string.Empty,
        };
    }

    public static string FormatDateTime(DateTime utc) =>
        BusinessTime.ToLocal(utc).ToString(DateTimeFormat, ViCulture);

    public static string FormatDate(DateOnly date) => date.ToString(DateFormat, ViCulture);

    private static string FormatNumber(object value) =>
        value is IConvertible
            ? Convert.ToDecimal(value, CultureInfo.InvariantCulture).ToString(NumberFormat, ViCulture)
            : Convert.ToString(value, ViCulture) ?? string.Empty;

    private static string FormatDate(object value, string format) =>
        value switch
        {
            DateTime dt => BusinessTime.ToLocal(dt).ToString(format, ViCulture),
            DateTimeOffset dto => BusinessTime.ToLocal(dto.UtcDateTime).ToString(format, ViCulture),
            DateOnly d => d.ToString(format, ViCulture),
            _ => Convert.ToString(value, ViCulture) ?? string.Empty,
        };
}
