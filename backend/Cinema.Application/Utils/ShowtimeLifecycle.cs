namespace Cinema.Application.Utils;

public static class ShowtimeLifecycle
{
    public const int MovieEndGraceDays = 1;

    public static bool IsOnlineSalesClosed(DateTime now, DateTime startTime, int stopMinutes) =>
        stopMinutes > 0 && now >= startTime.AddMinutes(-stopMinutes);

    public static DateTime MovieEndCutoff(DateTime now) => now.AddDays(-MovieEndGraceDays);
}
