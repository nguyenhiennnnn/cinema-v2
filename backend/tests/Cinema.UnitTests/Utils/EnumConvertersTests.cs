using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class EnumConvertersTests
{
    [Theory]
    [InlineData(BookingStatus.Pending, "Pending")]
    [InlineData(BookingStatus.Confirmed, "Confirmed")]
    [InlineData(BookingStatus.Cancelled, "Cancelled")]
    [InlineData(BookingStatus.Expired, "Expired")]
    public void ToDtoStatus_BookingStatus_MapsToExpectedString(
        BookingStatus status,
        string expected
    )
    {
        status.ToDtoStatus().ShouldBe(expected);
    }

    [Theory]
    [InlineData(PaymentStatus.Pending, "Pending")]
    [InlineData(PaymentStatus.Success, "Success")]
    [InlineData(PaymentStatus.Failed, "Failed")]
    [InlineData(PaymentStatus.Refunded, "Refunded")]
    public void ToDtoStatus_PaymentStatus_MapsToExpectedString(
        PaymentStatus status,
        string expected
    )
    {
        status.ToDtoStatus().ShouldBe(expected);
    }

    [Theory]
    [InlineData(BookingChannel.Online, "Online")]
    [InlineData(BookingChannel.Counter, "Counter")]
    public void ToDtoChannel_MapsToExpectedString(BookingChannel channel, string expected)
    {
        channel.ToDtoChannel().ShouldBe(expected);
    }

    [Fact]
    public void ParseEnum_ValidValue_IsCaseInsensitive()
    {
        EnumConverters.ParseEnum<BookingStatus>("pending").ShouldBe(BookingStatus.Pending);
        EnumConverters.ParseEnum<BookingStatus>("PENDING").ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public void ParseEnum_InvalidValue_ThrowsValidationAppExceptionWithFieldNamedError()
    {
        var ex = Should.Throw<ValidationAppException>(() =>
            EnumConverters.ParseEnum<BookingStatus>("not-a-status", "status")
        );

        ex.Errors.ShouldNotBeNull();
        ex.Errors!.ShouldContainKey("status");
    }

    [Fact]
    public void ParseNullableEnum_NullOrWhitespace_ReturnsNull()
    {
        EnumConverters.ParseNullableEnum<BookingStatus>(null).ShouldBeNull();
        EnumConverters.ParseNullableEnum<BookingStatus>("   ").ShouldBeNull();
    }

    [Fact]
    public void ParseNullableEnum_InvalidValue_ReturnsNullInsteadOfThrowing()
    {
        EnumConverters.ParseNullableEnum<BookingStatus>("garbage").ShouldBeNull();
    }

    [Fact]
    public void ParseNullableEnum_ValidValue_ReturnsParsedEnum()
    {
        EnumConverters
            .ParseNullableEnum<BookingStatus>("Confirmed")
            .ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public void TryParseEnum_ValidValue_ReturnsTrueAndParsedValue()
    {
        var success = EnumConverters.TryParseEnum<PaymentStatus>("Success", out var result);

        success.ShouldBeTrue();
        result.ShouldBe(PaymentStatus.Success);
    }

    [Fact]
    public void TryParseEnum_NullOrEmptyValue_ReturnsFalse()
    {
        EnumConverters.TryParseEnum<PaymentStatus>(null, out _).ShouldBeFalse();
        EnumConverters.TryParseEnum<PaymentStatus>("", out _).ShouldBeFalse();
    }
}
