using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class CodeGeneratorTests
{
    [Fact]
    public void GenerateBookingCode_StartsWithBKPrefixAndTodaysDate()
    {
        var code = CodeGenerator.GenerateBookingCode();

        code.ShouldStartWith("BK" + DateTime.UtcNow.ToString("yyyyMMdd"));
        code.Length.ShouldBe(2 + 8 + 4);
    }

    [Fact]
    public void GenerateBookingCode_SuffixOnlyContainsUppercaseLettersAndDigits()
    {
        var code = CodeGenerator.GenerateBookingCode();
        var suffix = code[^4..];

        suffix.ShouldAllBe(c => char.IsUpper(c) || char.IsDigit(c));
    }

    [Fact]
    public void GenerateBookingCode_IsHighlyUnlikelyToCollideAcrossManyCalls()
    {
        var codes = Enumerable
            .Range(0, 500)
            .Select(_ => CodeGenerator.GenerateBookingCode())
            .ToList();

        codes.Distinct().Count().ShouldBe(codes.Count);
    }

    [Theory]
    [InlineData("BK20260101ABCD", "A", 12, "BK20260101ABCD-A12")]
    [InlineData("BK20260101ABCD", "H", 1, "BK20260101ABCD-H1")]
    public void GenerateTicketQrPayload_ConcatenatesBookingCodeAndSeat(
        string bookingCode,
        string seatRow,
        int seatNumber,
        string expected
    )
    {
        CodeGenerator.GenerateTicketQrPayload(bookingCode, seatRow, seatNumber).ShouldBe(expected);
    }

    [Fact]
    public void GeneratePaymentTransactionId_StartsWithBookingCodeFollowedByUnixTimestamp()
    {
        const string bookingCode = "BK20260101ABCD";

        var transactionId = CodeGenerator.GeneratePaymentTransactionId(bookingCode);

        transactionId.ShouldStartWith(bookingCode);
        var suffix = transactionId[bookingCode.Length..];
        long.TryParse(suffix, out _)
            .ShouldBeTrue("the remainder must be a parseable unix timestamp");
    }
}
