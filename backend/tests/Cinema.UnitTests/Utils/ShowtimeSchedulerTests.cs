using Cinema.Application.Options;
using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class ShowtimeSchedulerTests
{
    private static readonly DateOnly Day = new(2026, 10, 28);
    private static readonly TimeOnly Open = new(8, 0);
    private static readonly TimeOnly Close = new(23, 59);
    private static readonly SchedulerRoom[] Rooms = [new(1, 120), new(2, 80), new(3, 60)];

    private static DateTime LocalAt(int day, TimeOnly time) =>
        BusinessTime.ToUtc(Day.AddDays(day), time);

    private static List<SchedulerSlot> Build(
        IReadOnlyList<SchedulerMovie> movies,
        IReadOnlyList<SchedulerBusy>? busy = null,
        IReadOnlyList<SchedulerRoom>? rooms = null,
        bool allowAfterHours = false
    ) =>
        ShowtimeScheduler.Build(
            Day,
            Open,
            Close,
            allowAfterHours,
            15,
            rooms ?? Rooms,
            movies,
            busy ?? []
        );

    private static bool Overlap(SchedulerSlot a, SchedulerSlot b) =>
        a.RoomId == b.RoomId && a.Start < b.End && b.Start < a.End;

    [Fact]
    public void Build_PlacesRequestedShowsWithoutOverlapAndWithinOpeningHours()
    {
        var slots = Build([new(10, 120, 0.9, 5), new(11, 100, 0.4, 4), new(12, 90, 0.1, 3)]);

        slots.Count(s => s.MovieId == 10).ShouldBe(5);
        for (var i = 0; i < slots.Count; i++)
        for (var j = i + 1; j < slots.Count; j++)
            Overlap(slots[i], slots[j]).ShouldBeFalse();

        var openUtc = LocalAt(0, Open);
        var closeUtc = LocalAt(0, Close);
        slots.ShouldAllBe(s => s.Start >= openUtc && s.End.AddMinutes(-15) <= closeUtc);
    }

    [Fact]
    public void Build_HotMovieGetsLargerRoomThanColdMovie()
    {
        var slots = Build(
            [new(1, 100, 0.1, 1), new(2, 100, 1.0, 1)],
            rooms: [new(1, 60), new(2, 120)]
        );

        slots.Single(s => s.MovieId == 2).RoomId.ShouldBe(2);
        slots.Single(s => s.MovieId == 1).RoomId.ShouldBe(1);
    }

    [Fact]
    public void Build_SpreadsShowsOfOneMovieAcrossTheDay()
    {
        var slots = Build([new(1, 120, 1, 3)], rooms: [new(1, 100)]);

        slots.Count.ShouldBe(3);
        for (var i = 1; i < slots.Count; i++)
            (slots[i].Start - slots[i - 1].Start).TotalMinutes.ShouldBeGreaterThanOrEqualTo(240);
    }

    [Fact]
    public void Build_RespectsExistingShowtimes()
    {
        var busyStart = LocalAt(0, new TimeOnly(8, 0));
        var busyEnd = LocalAt(0, new TimeOnly(23, 0));
        SchedulerBusy[] busy = [new(1, busyStart, busyEnd)];

        var slots = Build([new(1, 120, 0.5, 3)], busy, [new(1, 100)]);

        slots.Count.ShouldBeLessThanOrEqualTo(1);
        slots.ShouldAllBe(s => s.Start >= busyEnd);
    }

    [Fact]
    public void Build_MovieLongerThanWindow_PlacesNothing() =>
        ShowtimeScheduler
            .Build(
                Day,
                new TimeOnly(10, 0),
                new TimeOnly(11, 0),
                false,
                15,
                Rooms,
                [new(1, 120, 1, 3)],
                []
            )
            .ShouldBeEmpty();

    [Fact]
    public void Build_AllowAfterHours_AllowsEndPastClosingButNotStart()
    {
        var slots = Build([new(1, 120, 1, 20)], rooms: [new(1, 100)], allowAfterHours: true);

        var closeUtc = LocalAt(0, Close);
        slots.ShouldAllBe(s => s.Start < closeUtc);
        slots.Max(s => s.End).ShouldBeGreaterThan(closeUtc);
    }

    [Fact]
    public void Build_UsesConfiguredStepMinutes()
    {
        var slots = ShowtimeScheduler.Build(
            Day,
            Open,
            Close,
            false,
            15,
            Rooms,
            [new(10, 100, 0.8, 4)],
            [],
            new SchedulerOptions { StepMinutes = 15 }
        );

        slots.ShouldNotBeEmpty();
        slots.ShouldAllBe(s => s.Start.Minute % 15 == 0);
    }

    [Theory]
    [InlineData(0, 0, 5, 0.3)]
    [InlineData(0, 0, 2.5, 0.15)]
    [InlineData(10, 10, 5, 1.0)]
    [InlineData(10, 10, 0, 0.7)]
    public void Heat_CalculatesHeatCorrectlyOn1To5RatingScale(
        int tickets,
        int maxTickets,
        decimal rating,
        double expectedHeat
    )
    {
        var heat = ShowtimeScheduler.Heat(tickets, maxTickets, rating);
        heat.ShouldBe(expectedHeat, 0.001);
    }
}
