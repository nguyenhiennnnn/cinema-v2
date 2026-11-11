using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

[Collection(BusinessTimeGlobalStateCollection.Name)]
public class BusinessTimeTests
{
    private static void WithZone(string zoneId, Action action)
    {
        var previous = BusinessTime.IanaId;
        BusinessTime.Configure(zoneId);
        try
        {
            action();
        }
        finally
        {
            BusinessTime.Configure(previous);
        }
    }

    [Fact]
    public void IanaId_ConfiguredZone_IsIanaFormat() =>
        WithZone("Asia/Ho_Chi_Minh", () => BusinessTime.IanaId.ShouldBe("Asia/Ho_Chi_Minh"));

    [Fact]
    public void ToUtc_NonExistentLocalTime_ShiftsForwardInsteadOfThrowing() =>
        WithZone(
            "America/New_York",
            () =>
            {
                // 2027-03-14 02:30 không tồn tại ở New_York (nhảy từ 02:00 lên 03:00).
                var utc = BusinessTime.ToUtc(new DateOnly(2027, 3, 14), new TimeOnly(2, 30));

                utc.Kind.ShouldBe(DateTimeKind.Utc);
                utc.ShouldBe(new DateTime(2027, 3, 14, 7, 30, 0, DateTimeKind.Utc));
            }
        );

    [Fact]
    public void ToUtc_LocalKind_IsConvertedNotReinterpreted()
    {
        var local = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Local);

        BusinessTime.ToUtc(local).ShouldBe(local.ToUniversalTime());
    }
}
