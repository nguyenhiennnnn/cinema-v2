namespace Cinema.Application.Utils;

public static class ShowtimeLifecycle
{
    public const int DefaultMovieEndGraceDays = Domain.Entities.CinemaSettings.DefaultMovieEndGraceDays;

    public static bool IsOnlineSalesClosed(DateTime now, DateTime startTime, int stopMinutes) =>
        stopMinutes > 0 && now >= startTime.AddMinutes(-stopMinutes);

    public static DateTime MovieEndCutoff(DateTime now, int graceDays = DefaultMovieEndGraceDays) =>
        now.AddDays(-graceDays);
}
