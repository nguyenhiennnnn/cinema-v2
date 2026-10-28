using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class OperatingHoursTests
{
    private static readonly TimeOnly Open = new(8, 0);
    private static readonly TimeOnly Close = new(23, 59);
    private static DateTime At(int day, int hour, int minute = 0) =>
        new DateTime(2026, 10, 28).AddDays(day).AddHours(hour).AddMinutes(minute);

    [Fact]
    public void Violation_StartsBeforeOpening_ReturnsMessage() =>
        OperatingHours.Violation(At(0, 7, 59), At(0, 10), Open, Close, false).ShouldNotBeNull();

    [Fact]
    public void Violation_EndsExactlyAtClosing_ReturnsNull() =>
        OperatingHours.Violation(At(0, 21, 59), At(0, 23, 59), Open, Close, false).ShouldBeNull();

    [Fact]
    public void Violation_CrossesMidnightPastClosing_ReturnsMessageUnlessAllowed()
    {
        OperatingHours.Violation(At(0, 22, 30), At(1, 0, 30), Open, Close, false).ShouldNotBeNull();
        OperatingHours.Violation(At(0, 22, 30), At(1, 0, 30), Open, Close, true).ShouldBeNull();
    }

    [Fact]
    public void Violation_AllowedAfterHoursButStartsAtClosing_ReturnsMessage() =>
        OperatingHours.Violation(At(0, 23, 59), At(1, 2), Open, Close, true).ShouldNotBeNull();

    [Fact]
    public void Violation_OvernightWindow_AcceptsStartAfterMidnightOfPreviousDay()
    {
        var opening = new TimeOnly(10, 0);
        var closing = new TimeOnly(2, 0);

        OperatingHours.Violation(At(1, 0, 30), At(1, 2), opening, closing, false).ShouldBeNull();
        OperatingHours.Violation(At(1, 0, 30), At(1, 2, 1), opening, closing, false).ShouldNotBeNull();
        OperatingHours.Violation(At(1, 3), At(1, 5), opening, closing, false).ShouldNotBeNull();
    }

    [Fact]
    public void Violation_NoSettings_ReturnsNull() =>
        OperatingHours.Violation(At(0, 1), At(0, 3), null, null, false).ShouldBeNull();
}
