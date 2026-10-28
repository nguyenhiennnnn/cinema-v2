using Cinema.Application.Utils;
using Shouldly;

namespace Cinema.UnitTests.Utils;

public class ShowtimeSchedulerTests
{
    private static readonly DateOnly Day = new(2026, 10, 28);
    private static readonly TimeOnly Open = new(8, 0);
    private static readonly TimeOnly Close = new(23, 59);
    private static readonly SchedulerRoom[] Rooms = [new(1, 120), new(2, 80), new(3, 60)];

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
        slots.ShouldAllBe(s =>
            s.Start >= Day.ToDateTime(Open) && s.End.AddMinutes(-15) <= Day.ToDateTime(Close)
        );
    }

    [Fact]
    public void Build_HotMovieGetsPrimeSlotBeforeColdMovie()
    {
        var slots = Build([new(1, 120, 0.1, 1), new(2, 120, 0.9, 1)], rooms: [new(1, 100)]);

        var hot = slots.Single(s => s.MovieId == 2);
        ShowtimeScheduler.Prime(hot.Start, Day).ShouldBe(1);
    }

    [Fact]
    public void Build_RespectsExistingShowtimes()
    {
        SchedulerBusy[] busy = [new(1, Day.ToDateTime(new TimeOnly(8, 0)), Day.ToDateTime(new TimeOnly(23, 0)))];

        var slots = Build([new(1, 120, 0.5, 3)], busy, [new(1, 100)]);

        slots.Count.ShouldBeLessThanOrEqualTo(1);
        slots.ShouldAllBe(s => s.Start >= Day.ToDateTime(new TimeOnly(23, 0)));
    }

    [Fact]
    public void Build_MovieLongerThanWindow_PlacesNothing() =>
        ShowtimeScheduler
            .Build(Day, new TimeOnly(10, 0), new TimeOnly(11, 0), false, 15, Rooms, [new(1, 120, 1, 3)], [])
            .ShouldBeEmpty();

    [Fact]
    public void Build_AllowAfterHours_AllowsEndPastClosingButNotStart()
    {
        var slots = Build([new(1, 120, 1, 20)], rooms: [new(1, 100)], allowAfterHours: true);

        slots.ShouldAllBe(s => s.Start < Day.ToDateTime(Close));
        slots.Max(s => s.End).ShouldBeGreaterThan(Day.ToDateTime(Close));
    }

    [Fact]
    public void PriceFactor_PrimeTimeCostsMoreThanMorning()
    {
        var prime = ShowtimeScheduler.PriceFactor(Day.ToDateTime(new TimeOnly(19, 0)), Day, 0.5);
        var morning = ShowtimeScheduler.PriceFactor(Day.ToDateTime(new TimeOnly(9, 0)), Day, 0.5);

        prime.ShouldBeGreaterThan(morning);
    }

    [Fact]
    public void PriceFactor_WeekendAndHotMovieCostMore()
    {
        var weekday = new DateOnly(2026, 10, 28);
        var saturday = new DateOnly(2026, 10, 31);
        var at = new TimeOnly(19, 0);

        ShowtimeScheduler
            .PriceFactor(saturday.ToDateTime(at), saturday, 0.5)
            .ShouldBeGreaterThan(ShowtimeScheduler.PriceFactor(weekday.ToDateTime(at), weekday, 0.5));
        ShowtimeScheduler
            .PriceFactor(weekday.ToDateTime(at), weekday, 1)
            .ShouldBeGreaterThan(ShowtimeScheduler.PriceFactor(weekday.ToDateTime(at), weekday, 0));
    }

    [Fact]
    public void PriceFactor_IsClamped()
    {
        var saturday = new DateOnly(2026, 10, 31);

        ShowtimeScheduler
            .PriceFactor(saturday.ToDateTime(new TimeOnly(19, 0)), saturday, 1)
            .ShouldBeLessThanOrEqualTo(1.4);
    }

    [Theory]
    [InlineData(75000, 1.15, 85000)]
    [InlineData(75000, 0.85, 65000)]
    [InlineData(75000, 1.0, 75000)]
    [InlineData(3000, 1.3, 3000)]
    public void Price_RoundsToStep(decimal basePrice, double factor, decimal expected) =>
        ShowtimeScheduler.Price(basePrice, factor).ShouldBe(expected);
}
