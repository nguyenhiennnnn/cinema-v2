namespace Cinema.Application.Utils;

public static class VietnamTime
{
    private static readonly TimeZoneInfo Zone = ResolveZone();

    public static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(
                TimeZoneInfo.ConvertTimeToUtc(value, Zone),
                DateTimeKind.Utc
            ),
        };

    public static DateTime ToUtc(DateOnly day, TimeOnly time) =>
        ToUtc(DateTime.SpecifyKind(day.ToDateTime(time), DateTimeKind.Unspecified));

    public static DateTime ToLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(AsUtc(utc), Zone);

    public static DateOnly LocalDate(DateTime utc) => DateOnly.FromDateTime(ToLocal(utc));

    public static DateOnly Today(DateTime? utcNow = null) =>
        LocalDate(utcNow ?? DateTime.UtcNow);

    public static DateTime StartOfDayUtc(DateOnly localDay) => ToUtc(localDay, TimeOnly.MinValue);

    public static DateTime StartOfNextDayUtc(DateOnly localDay) =>
        StartOfDayUtc(localDay.AddDays(1));

    private static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    private static TimeZoneInfo ResolveZone()
    {
        foreach (var id in new[] { "Asia/Ho_Chi_Minh", "SE Asia Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }

        return TimeZoneInfo.CreateCustomTimeZone(
            "Asia/Ho_Chi_Minh",
            TimeSpan.FromHours(7),
            "Asia/Ho_Chi_Minh",
            "Asia/Ho_Chi_Minh"
        );
    }
}
