using System.Globalization;
using Cinema.Application.Interfaces;

namespace Cinema.Application.Utils;

public static class PdfCellFormatter
{
    private const string NumberFormat = "#,##0.##";
    private const string DateFormat = "dd/MM/yyyy";
    private const string DateTimeFormat = "dd/MM/yyyy HH:mm";

    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    private static readonly NumberFormatInfo VietnameseNumbers = new()
    {
        NumberGroupSeparator = ".",
        NumberDecimalSeparator = ",",
    };

    public static string Format(object? value, ExportColumnType type)
    {
        if (value is null)
            return string.Empty;

        return type switch
        {
            ExportColumnType.Number or ExportColumnType.Money => FormatNumber(value),
            ExportColumnType.Date => FormatDate(value, DateFormat),
            ExportColumnType.DateTime => FormatDate(value, DateTimeFormat),
            _ => Convert.ToString(value, Culture) ?? string.Empty,
        };
    }

    public static string FormatDateTime(DateTime utc) =>
        BusinessTime.ToLocal(utc).ToString(DateTimeFormat, Culture);

    public static string FormatDate(DateOnly date) => date.ToString(DateFormat, Culture);

    private static string FormatNumber(object value) =>
        value is IConvertible
            ? Convert.ToDecimal(value, Culture).ToString(NumberFormat, VietnameseNumbers)
            : Convert.ToString(value, Culture) ?? string.Empty;

    private static string FormatDate(object value, string format) =>
        value switch
        {
            DateTime dt => BusinessTime.ToLocal(dt).ToString(format, Culture),
            DateTimeOffset dto => BusinessTime.ToLocal(dto.UtcDateTime).ToString(format, Culture),
            DateOnly d => d.ToString(format, Culture),
            _ => Convert.ToString(value, Culture) ?? string.Empty,
        };
}
