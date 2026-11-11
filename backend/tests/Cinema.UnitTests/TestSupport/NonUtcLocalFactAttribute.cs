namespace Cinema.UnitTests.TestSupport;

public sealed class NonUtcLocalFactAttribute : FactAttribute
{
    public NonUtcLocalFactAttribute()
    {
        if (TimeZoneInfo.Local.GetUtcOffset(new DateTime(2026, 3, 15, 10, 0, 0, DateTimeKind.Utc)) == TimeSpan.Zero)
            Skip = "Cần chạy với TZ khác UTC, ví dụ: TZ=Asia/Ho_Chi_Minh dotnet test";
    }
}
