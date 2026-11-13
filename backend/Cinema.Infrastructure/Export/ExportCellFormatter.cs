using System.Globalization;
using Cinema.Application.Utils;

namespace Cinema.Infrastructure.Export;

internal static class ExportCellFormatter
{
    private static readonly char[] FormulaPrefixes = ['=', '+', '-', '@'];

    public static string? ToSafeText(object? value)
    {
        if (value is null)
            return null;

        var text = value is IFormattable formattable
            ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : value.ToString();

        if (string.IsNullOrEmpty(text))
            return text;

        return Array.IndexOf(FormulaPrefixes, text[0]) >= 0 ? "'" + text : text;
    }

    public static decimal? ToDecimal(object? value) =>
        value is null
            ? null
            : value is decimal d
                ? d
                : Convert.ToDecimal(value, CultureInfo.InvariantCulture);

    public static bool HasFraction(decimal value) => value != decimal.Truncate(value);

    public static DateTime? ToLocalDateTime(object? value) =>
        value switch
        {
            null => null,
            DateTime dt => BusinessTime.ToLocal(dt),
            DateTimeOffset dto => BusinessTime.ToLocal(dto.UtcDateTime),
            DateOnly d => d.ToDateTime(TimeOnly.MinValue),
            _ => null,
        };

    public static string FormatNumber(decimal value) =>
        value.ToString("0.############", CultureInfo.InvariantCulture);

    public static string FormatMoney(decimal value) =>
        value.ToString("0.##", CultureInfo.InvariantCulture);

    public static string FormatDate(DateTime value) =>
        value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string FormatDateTime(DateTime value) =>
        value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
