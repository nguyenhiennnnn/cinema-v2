namespace Cinema.Application.Utils;

public static class OperatingHours
{
    public static (DateTime Open, DateTime? Close) Window(
        DateOnly day,
        TimeOnly? opening,
        TimeOnly? closing
    )
    {
        var open = day.ToDateTime(opening ?? TimeOnly.MinValue);
        if (closing is null)
            return (open, null);

        var close = day.ToDateTime(closing.Value);
        if (closing.Value <= (opening ?? TimeOnly.MinValue))
            close = close.AddDays(1);

        return (open, close);
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

        var startDay = DateOnly.FromDateTime(start);
        var days = closing is null ? new[] { startDay } : new[] { startDay, startDay.AddDays(-1) };

        foreach (var day in days)
        {
            var (open, close) = Window(day, opening, closing);
            if (start < open)
                continue;
            if (close is null || (allowAfterHours ? start < close : end <= close))
                return null;
        }

        var range = $"{opening?.ToString("HH:mm") ?? "--"} - {closing?.ToString("HH:mm") ?? "--"}";
        return $"Suất chiếu {start:HH:mm} - {end:HH:mm} nằm ngoài khung giờ hoạt động của rạp ({range}).";
    }
}
