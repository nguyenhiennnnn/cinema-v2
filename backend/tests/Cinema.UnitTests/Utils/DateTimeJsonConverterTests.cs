using System.Text.Json;
using Cinema.Application.Utils;
using Cinema.UnitTests.TestSupport;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class DateTimeJsonConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters = { new DateTimeJsonConverter() },
    };

    private static readonly DateTime Instant = new(2026, 3, 15, 10, 30, 45, 123, DateTimeKind.Utc);
    private const string ExpectedJson = "\"2026-03-15T10:30:45.123Z\"";

    [Fact]
    public void Write_KindUtc_KeepsValue() =>
        JsonSerializer.Serialize(Instant, Options).ShouldBe(ExpectedJson);

    [Fact]
    public void Write_KindUnspecified_IsTreatedAsUtc()
    {
        var unspecified = DateTime.SpecifyKind(Instant, DateTimeKind.Unspecified);

        JsonSerializer.Serialize(unspecified, Options).ShouldBe(ExpectedJson);
    }

    [Fact]
    public void Write_KindLocal_ConvertsToUniversalTime()
    {
        var local = Instant.ToLocalTime();

        local.Kind.ShouldBe(DateTimeKind.Local);
        JsonSerializer.Serialize(local, Options).ShouldBe(ExpectedJson);
    }

    [NonUtcLocalFact]
    public void Write_KindLocal_WhenServerNotUtc_DoesNotShiftWallClock()
    {
        var local = Instant.ToLocalTime();

        local.TimeOfDay.ShouldNotBe(Instant.TimeOfDay);
        JsonSerializer.Serialize(local, Options).ShouldBe(ExpectedJson);
    }

    [NonUtcLocalFact]
    public void Write_KindUnspecified_WhenServerNotUtc_IsNotInterpretedAsLocal()
    {
        var unspecified = DateTime.SpecifyKind(Instant, DateTimeKind.Unspecified);

        JsonSerializer.Serialize(unspecified, Options).ShouldBe(ExpectedJson);
    }

    [Fact]
    public void Write_LocalAndUtcOfSameInstant_ProduceSameJson()
    {
        var utc = new DateTime(2026, 7, 1, 23, 59, 59, DateTimeKind.Utc);

        JsonSerializer.Serialize(utc.ToLocalTime(), Options).ShouldBe(JsonSerializer.Serialize(utc, Options));
    }
}
