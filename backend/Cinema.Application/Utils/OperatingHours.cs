using System.Globalization;
using Cinema.Shared.Resources;

namespace Cinema.Application.Utils;

public static class OperatingHours
{
    public static (DateTime Open, DateTime? Close) Window(
        DateOnly day,
        TimeOnly? opening,
        TimeOnly? closing
    )
    {
        var open = BusinessTime.ToUtc(day, opening ?? TimeOnly.MinValue);
        if (closing is null)
            return (open, null);

        var closeDay = closing.Value <= (opening ?? TimeOnly.MinValue) ? day.AddDays(1) : day;
        return (open, BusinessTime.ToUtc(closeDay, closing.Value));
    }

    public static string? Violation(
        DateTime start,
        DateTime end,
        TimeOnly? opening,
        TimeOnly? closing,
        bool allowAfterHours
    )
    {
        if (opening is null && closing is null)
            return null;

        var startDay = BusinessTime.LocalDate(start);
        var days = closing is null ? new[] { startDay } : new[] { startDay, startDay.AddDays(-1) };

        foreach (var day in days)
        {
            var (open, close) = Window(day, opening, closing);
            if (start < open)
                continue;
            if (close is null || (allowAfterHours ? start < close : end <= close))
                return null;
        }

        var range =
            $"{opening?.ToString("HH:mm", CultureInfo.InvariantCulture) ?? "--"} - {closing?.ToString("HH:mm", CultureInfo.InvariantCulture) ?? "--"}";
        var startText = BusinessTime.ToLocal(start).ToString("HH:mm", CultureInfo.InvariantCulture);
        var endText = BusinessTime.ToLocal(end).ToString("HH:mm", CultureInfo.InvariantCulture);
        return string.Format(
            Messages.Rule_ShowtimeOutsideOperatingHours,
            startText,
            endText,
            range
        );
    }
}
