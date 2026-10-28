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
    public void Build_HotMovieGetsLargerRoomThanColdMovie()
    {
        var slots = Build([new(1, 100, 0.1, 1), new(2, 100, 1.0, 1)], rooms: [new(1, 60), new(2, 120)]);

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
}
