namespace Cinema.Application.Utils;

public static class VietnamTime
{
    private static TimeZoneInfo _zone = ResolveZone("Asia/Ho_Chi_Minh")!;

    public static void Configure(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
            throw new InvalidOperationException(
                "Brand:TimeZoneId không được để trống. Kiểm tra appsettings.json."
            );

        _zone =
            ResolveZone(timeZoneId)
            ?? throw new InvalidOperationException(
                $"Timezone '{timeZoneId}' (Brand:TimeZoneId) không hợp lệ hoặc không được hỗ trợ trên hệ thống này. "
                    + "Dùng IANA ID (ví dụ: 'Asia/Ho_Chi_Minh') hoặc Windows ID ('SE Asia Standard Time')."
            );
    }

    public static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(
                TimeZoneInfo.ConvertTimeToUtc(value, _zone),
                DateTimeKind.Utc
            ),
        };

    public static DateTime ToUtc(DateOnly day, TimeOnly time) =>
        ToUtc(DateTime.SpecifyKind(day.ToDateTime(time), DateTimeKind.Unspecified));

    public static DateTime ToLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(AsUtc(utc), _zone);

    public static DateOnly LocalDate(DateTime utc) => DateOnly.FromDateTime(ToLocal(utc));

    public static DateOnly Today(DateTime? utcNow = null) => LocalDate(utcNow ?? DateTime.UtcNow);

    public static DateTime StartOfDayUtc(DateOnly localDay) => ToUtc(localDay, TimeOnly.MinValue);

    public static DateTime StartOfNextDayUtc(DateOnly localDay) =>
        StartOfDayUtc(localDay.AddDays(1));

    private static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    private static TimeZoneInfo? ResolveZone(string id)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
        catch (TimeZoneNotFoundException) { }
        catch (InvalidTimeZoneException) { }

        if (TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out var winId))
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(winId);
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }

        if (TimeZoneInfo.TryConvertWindowsIdToIanaId(id, out var ianaId))
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
            }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }

        return null;
    }
}
