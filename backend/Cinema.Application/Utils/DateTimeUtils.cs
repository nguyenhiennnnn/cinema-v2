using System.Globalization;

namespace Cinema.Application.Utils;

public static class DateTimeUtils
{
    public static TimeOnly? ParseTimeOnly(string? value, string format = "HH:mm")
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return TimeOnly.ParseExact(value.Trim(), format, CultureInfo.InvariantCulture);
    }

    public static string FormatDuration(TimeSpan span) =>
        span.TotalMinutes >= 1
            ? $"{(int)Math.Ceiling(span.TotalMinutes)} phút"
            : $"{(int)Math.Ceiling(span.TotalSeconds)} giây";

    public static string GetPeriodKey(DateTime createdAt, string groupBy) =>
        groupBy switch
        {
            "day" => createdAt.ToString("dd/MM/yyyy"),
            "week" => $"{ISOWeek.GetYear(createdAt)}-W{ISOWeek.GetWeekOfYear(createdAt):D2}",
            "month" => createdAt.ToString("MM/yyyy"),
            _ => createdAt.ToString("dd/MM/yyyy"),
        };

    public static DateTime GetStartOfDay(DateOnly date) =>
        date.ToDateTime(TimeOnly.MinValue);

    public static DateTime GetStartOfNextDay(DateOnly date) =>
        date.AddDays(1).ToDateTime(TimeOnly.MinValue);
}
