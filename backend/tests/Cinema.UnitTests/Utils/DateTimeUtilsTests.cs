using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class DateTimeUtilsTests
{
    [Fact]
    public void ParseTimeOnly_NullOrWhitespace_ReturnsNull()
    {
        DateTimeUtils.ParseTimeOnly(null).ShouldBeNull();
        DateTimeUtils.ParseTimeOnly("   ").ShouldBeNull();
    }

    [Fact]
    public void ParseTimeOnly_ValidHHmm_ParsesCorrectly()
    {
        var result = DateTimeUtils.ParseTimeOnly("09:30");

        result.ShouldBe(new TimeOnly(9, 30));
    }

    [Fact]
    public void ParseTimeOnly_InvalidFormat_Throws()
    {
        Should.Throw<FormatException>(() => DateTimeUtils.ParseTimeOnly("9:30am"));
    }

    [Theory]
    [InlineData(90, "2 phút")]
    [InlineData(60, "1 phút")]
    [InlineData(30, "30 giây")]
    public void FormatDuration_FormatsMinutesOrSecondsBasedOnLength(int seconds, string expected)
    {
        DateTimeUtils.FormatDuration(TimeSpan.FromSeconds(seconds)).ShouldBe(expected);
    }

    [Fact]
    public void GetPeriodKey_Day_FormatsAsDdMmYyyy()
    {
        var date = new DateTime(2026, 3, 5);

        DateTimeUtils.GetPeriodKey(date, "day").ShouldBe("05/03/2026");
    }

    [Fact]
    public void GetPeriodKey_Month_FormatsAsMmYyyy()
    {
        var date = new DateTime(2026, 3, 5);

        DateTimeUtils.GetPeriodKey(date, "month").ShouldBe("03/2026");
    }

    [Fact]
    public void GetPeriodKey_UnknownGroupBy_FallsBackToDayFormat()
    {
        var date = new DateTime(2026, 3, 5);

        DateTimeUtils.GetPeriodKey(date, "year").ShouldBe("05/03/2026");
    }

    [Fact]
    public void GetStartOfDay_ReturnsMidnightLocalDayAsUtc()
    {
        var date = new DateOnly(2026, 6, 15);

        DateTimeUtils.GetStartOfDay(date).ShouldBe(BusinessTime.StartOfDayUtc(date));
    }

    [Fact]
    public void GetStartOfNextDay_ReturnsMidnightNextLocalDayAsUtc()
    {
        var date = new DateOnly(2026, 6, 15);

        DateTimeUtils.GetStartOfNextDay(date).ShouldBe(BusinessTime.StartOfNextDayUtc(date));
    }
}
