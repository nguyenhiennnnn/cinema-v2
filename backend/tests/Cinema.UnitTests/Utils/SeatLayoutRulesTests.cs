using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class SeatLayoutRulesTests
{
    private static SeatSlot Seat(
        string row,
        int number,
        ShowtimeSeatStatus status = ShowtimeSeatStatus.Available,
        Guid? coupleId = null,
        decimal price = 100_000m
    ) => new(ShowtimeSeatId(row, number), row, number, 1, coupleId, status, price);

    private static int ShowtimeSeatId(string row, int number) => (row[0] - 'A' + 1) * 100 + number;

    private static List<SeatSlot> Row(string row, int count) =>
        Enumerable.Range(1, count).Select(n => Seat(row, n)).ToList();

    private static int[] Ids(string row, params int[] numbers) =>
        numbers.Select(n => ShowtimeSeatId(row, n)).ToArray();

    [Fact]
    public void ValidateSelection_NoMatchingSeats_ReturnsNull() =>
        SeatLayoutRules.ValidateSelection(Row("A", 4), [999]).ShouldBeNull();

    [Fact]
    public void ValidateSelection_Strict_PickLeavesSingleSeatBesideWall_ReturnsMessage()
    {
        var error = SeatLayoutRules.ValidateSelection(Row("A", 4), Ids("A", 2));

        error.ShouldNotBeNull();
        error.ShouldContain("A1");
    }

    [Fact]
    public void ValidateSelection_Strict_PickAtEdgeLeavingRunOfThree_ReturnsNull() =>
        SeatLayoutRules.ValidateSelection(Row("A", 4), Ids("A", 1)).ShouldBeNull();

    [Fact]
    public void ValidateSelection_Strict_PickTwoAtEdgeLeavingRunOfTwo_ReturnsNull() =>
        SeatLayoutRules.ValidateSelection(Row("A", 4), Ids("A", 1, 2)).ShouldBeNull();

    [Fact]
    public void ValidateSelection_Strict_PickSplitsRowIntoSingleSeatInTheMiddle_ReturnsMessage()
    {
        var error = SeatLayoutRules.ValidateSelection(
            Row("A", 5),
            Ids("A", 1, 3),
            SeatGapRuleMode.Strict
        );

        error.ShouldNotBeNull();
        error.ShouldContain("A2");
    }

    [Fact]
    public void ValidateSelection_AllowEdge_SingleSeatBesideWall_ReturnsNull() =>
        SeatLayoutRules
            .ValidateSelection(Row("A", 4), Ids("A", 2), SeatGapRuleMode.AllowEdge)
            .ShouldBeNull();

    [Fact]
    public void ValidateSelection_AllowEdge_SingleSeatBetweenPickedAndAvailableNeighbour_StillReturnsMessage()
    {
        var error = SeatLayoutRules.ValidateSelection(
            Row("A", 5),
            Ids("A", 1, 3),
            SeatGapRuleMode.AllowEdge
        );

        error.ShouldNotBeNull();
        error.ShouldContain("A2");
    }

    [Fact]
    public void ValidateSelection_Off_AllowsAnyGap() =>
        SeatLayoutRules
            .ValidateSelection(Row("A", 5), Ids("A", 1, 3), SeatGapRuleMode.Off)
            .ShouldBeNull();

    [Fact]
    public void ValidateSelection_ExistingSingleSeatNotAdjacentToPick_IsIgnored()
    {
        var seats = new List<SeatSlot>
        {
            Seat("A", 1),
            Seat("A", 2, ShowtimeSeatStatus.Booked),
            Seat("A", 3),
            Seat("A", 4),
            Seat("A", 5),
        };

        SeatLayoutRules.ValidateSelection(seats, Ids("A", 3)).ShouldBeNull();
    }

    [Fact]
    public void ValidateSelection_OnlyChecksRowsContainingPickedSeats()
    {
        var seats = Row("A", 4);
        seats.AddRange([Seat("B", 1), Seat("B", 2, ShowtimeSeatStatus.Booked), Seat("B", 3)]);

        SeatLayoutRules.ValidateSelection(seats, Ids("A", 1)).ShouldBeNull();
    }

    [Fact]
    public void ValidateSelection_CouplePickedPartially_ReturnsMessage()
    {
        var couple = Guid.NewGuid();
        var seats = new List<SeatSlot>
        {
            Seat("A", 1, coupleId: couple),
            Seat("A", 2, coupleId: couple),
            Seat("A", 3),
            Seat("A", 4),
        };

        SeatLayoutRules
            .ValidateSelection(seats, Ids("A", 1))
            .ShouldBe("Ghế đôi phải được chọn cùng nhau.");
    }

    [Fact]
    public void ValidateSelection_CouplePickedPartiallyEvenWhenGapRuleOff_ReturnsMessage()
    {
        var couple = Guid.NewGuid();
        var seats = new List<SeatSlot>
        {
            Seat("A", 1, coupleId: couple),
            Seat("A", 2, coupleId: couple),
            Seat("A", 3),
            Seat("A", 4),
        };

        SeatLayoutRules
            .ValidateSelection(seats, Ids("A", 2), SeatGapRuleMode.Off)
            .ShouldBe("Ghế đôi phải được chọn cùng nhau.");
    }

    [Fact]
    public void ValidateSelection_CouplePickedTogether_ReturnsNull()
    {
        var couple = Guid.NewGuid();
        var seats = new List<SeatSlot>
        {
            Seat("A", 1, coupleId: couple),
            Seat("A", 2, coupleId: couple),
            Seat("A", 3),
            Seat("A", 4),
        };

        SeatLayoutRules.ValidateSelection(seats, Ids("A", 1, 2)).ShouldBeNull();
    }

    [Fact]
    public void Suggest_ReturnsConsecutiveSeatsThatDoNotLeaveSingleSeatAtEdge()
    {
        var result = SeatLayoutRules.Suggest(Row("A", 4), cols: 4, count: 2);

        result.Count.ShouldBe(1);
        result[0].SeatRow.ShouldBe("A");
        result[0].SeatNumbers.Count.ShouldBe(2);
        result[0].SeatNumbers[1].ShouldBe(result[0].SeatNumbers[0] + 1);
        result[0].SeatNumbers.ShouldNotBe([2, 3]);
    }

    [Fact]
    public void Suggest_SumsPriceAndReturnsShowtimeSeatIds()
    {
        var seats = Enumerable.Range(1, 6).Select(n => Seat("A", n, price: 50_000m + n)).ToList();

        var result = SeatLayoutRules.Suggest(seats, cols: 6, count: 2);

        var suggestion = result.Single();
        suggestion.ShowtimeSeatIds.ShouldBe(
            suggestion.SeatNumbers.Select(n => ShowtimeSeatId("A", n)).ToList()
        );
        suggestion.TotalPrice.ShouldBe(suggestion.SeatNumbers.Sum(n => 50_000m + n));
    }

    [Fact]
    public void Suggest_NotEnoughConsecutiveSeats_ReturnsEmpty() =>
        SeatLayoutRules.Suggest(Row("A", 2), cols: 2, count: 3).ShouldBeEmpty();

    [Fact]
    public void Suggest_SkipsUnavailableSeats()
    {
        var seats = new List<SeatSlot>
        {
            Seat("A", 1, ShowtimeSeatStatus.Booked),
            Seat("A", 2, ShowtimeSeatStatus.Held),
            Seat("A", 3),
            Seat("A", 4),
            Seat("A", 5),
            Seat("A", 6),
        };

        var result = SeatLayoutRules.Suggest(seats, cols: 6, count: 2);

        result.Count.ShouldBe(1);
        result[0].SeatNumbers.ShouldAllBe(n => n >= 3);
    }

    [Fact]
    public void Suggest_LimitsResultsToTopAndOnePerRow()
    {
        var seats = new List<SeatSlot>();
        foreach (var row in new[] { "A", "B", "C", "D", "E" })
            seats.AddRange(Row(row, 6));

        var result = SeatLayoutRules.Suggest(seats, cols: 6, count: 2, top: 3);

        result.Count.ShouldBe(3);
        result.Select(r => r.SeatRow).Distinct().Count().ShouldBe(3);
        result.Select(r => r.Score).ShouldBe(result.Select(r => r.Score).OrderByDescending(s => s));
    }

    [Fact]
    public void Suggest_CoupleSeatsAreNeverSplitAcrossWindow()
    {
        var couple = Guid.NewGuid();
        var seats = new List<SeatSlot>
        {
            Seat("A", 1),
            Seat("A", 2),
            Seat("A", 3),
            Seat("A", 4, coupleId: couple),
            Seat("A", 5, coupleId: couple),
            Seat("A", 6),
            Seat("A", 7),
            Seat("A", 8),
        };

        var result = SeatLayoutRules.Suggest(seats, cols: 8, count: 3, top: 3);

        foreach (var suggestion in result)
        {
            var hasFour = suggestion.SeatNumbers.Contains(4);
            var hasFive = suggestion.SeatNumbers.Contains(5);
            hasFour.ShouldBe(hasFive);
        }
    }
}
