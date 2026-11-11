using Cinema.Application.DTOs.Showtimes;
using Cinema.Application.Utils;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class ShowtimeServiceAutoScheduleTests : ShowtimeServiceTestBase
{
    private static DateOnly TargetDay => BusinessTime.Today().AddDays(2);

    [Fact]
    public async Task CreateShowtimeAsync_EndsAfterClosing_ThrowsUnlessAllowAfterHours()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 120);
        var sut = CreateShowtimeService();
        var start = BusinessTime.ToUtc(TargetDay, new TimeOnly(22, 30));

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, start, 90_000m),
                CancellationToken.None
            )
        );

        var created = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, start, 90_000m, true),
            CancellationToken.None
        );
        created.ShowtimeId.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task PreviewAutoScheduleAsync_SchedulesAroundExistingShowtimeWithoutOverlap()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 4);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 120);
        var sut = CreateAutoScheduleService();
        var showtimeSut = CreateShowtimeService();
        var existing = await showtimeSut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(
                movie.MovieId,
                room.RoomId,
                BusinessTime.ToUtc(TargetDay, new TimeOnly(18, 0)),
                90_000m
            ),
            CancellationToken.None
        );

        var result = await sut.PreviewAutoScheduleAsync(
            new AutoScheduleRequest(
                TargetDay,
                [new AutoScheduleMovieRequest(movie.MovieId, 3)],
                90_000m
            ),
            CancellationToken.None
        );

        result.Items.Count.ShouldBe(3);
        result
            .Items.Where(i => i.RoomId == existing.RoomId)
            .ShouldAllBe(i => i.EndTime <= existing.StartTime || i.StartTime >= existing.EndTime);
        result.Unplaced.ShouldBeEmpty();
    }

    [Fact]
    public async Task PreviewAutoScheduleAsync_EndedMovie_ThrowsBusinessRuleException()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, status: Cinema.Domain.Enums.MovieStatus.Ended);
        var sut = CreateAutoScheduleService();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.PreviewAutoScheduleAsync(
                new AutoScheduleRequest(
                    TargetDay,
                    [new AutoScheduleMovieRequest(movie.MovieId, 1)],
                    90_000m
                ),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task ApplyAutoScheduleAsync_SkipsConflictingItemsAndCreatesTheRest()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 120);
        var sut = CreateAutoScheduleService();
        var first = BusinessTime.ToUtc(TargetDay, new TimeOnly(14, 0));

        var result = await sut.ApplyAutoScheduleAsync(
            new ApplyAutoScheduleRequest([
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, first, 90_000m),
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, first.AddHours(1), 90_000m),
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, first.AddHours(4), 90_000m),
            ]),
            CancellationToken.None
        );

        result.TotalCreated.ShouldBe(2);
        result.TotalSkipped.ShouldBe(1);
        (await Db.Showtimes.CountAsync()).ShouldBe(2);
    }
}
