using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class ShowtimeLifecycleServiceTests(PostgresContainerFixture fixture) : ShowtimeServiceTestBase(fixture), IClassFixture<PostgresContainerFixture>
{
    [Fact]
    public async Task CompleteEndedShowtimesAsync_OnlyScheduledShowtimesPastEndTimeBecomeCompleted()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 100);
        var past = TestDataSeeder.AddShowtime(Db, movie, room, seats, DateTime.UtcNow.AddHours(-5));
        var running = TestDataSeeder.AddShowtime(
            Db,
            movie,
            room,
            seats,
            DateTime.UtcNow.AddMinutes(-30)
        );
        var cancelled = TestDataSeeder.AddShowtime(
            Db,
            movie,
            room,
            seats,
            DateTime.UtcNow.AddHours(-8),
            status: ShowtimeStatus.Cancelled
        );
        var sut = CreateLifecycleService();

        var count = await sut.CompleteEndedShowtimesAsync(CancellationToken.None);

        count.ShouldBe(1);
        (
            await Db.Showtimes.AsNoTracking().SingleAsync(s => s.ShowtimeId == past.ShowtimeId)
        ).Status.ShouldBe(ShowtimeStatus.Completed);
        (
            await Db.Showtimes.AsNoTracking().SingleAsync(s => s.ShowtimeId == running.ShowtimeId)
        ).Status.ShouldBe(ShowtimeStatus.Scheduled);
        (
            await Db.Showtimes.AsNoTracking().SingleAsync(s => s.ShowtimeId == cancelled.ShowtimeId)
        ).Status.ShouldBe(ShowtimeStatus.Cancelled);
    }

    [Fact]
    public async Task EndMoviesWithoutUpcomingShowtimesAsync_EndsOnlyNowShowingMoviesWithNoRecentOrFutureShowtimes()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var stale = TestDataSeeder.AddMovie(Db, title: "Stale");
        var upcoming = TestDataSeeder.AddMovie(Db, title: "Upcoming");
        var neverScheduled = TestDataSeeder.AddMovie(Db, title: "Never");
        var comingSoon = TestDataSeeder.AddMovie(Db, title: "Soon", status: MovieStatus.ComingSoon);
        TestDataSeeder.AddShowtime(
            Db,
            stale,
            room,
            seats,
            DateTime.UtcNow.AddDays(-3),
            status: ShowtimeStatus.Completed
        );
        TestDataSeeder.AddShowtime(Db, upcoming, room, seats, DateTime.UtcNow.AddDays(1));
        var sut = CreateLifecycleService();

        var count = await sut.EndMoviesWithoutUpcomingShowtimesAsync(CancellationToken.None);

        count.ShouldBe(1);
        (
            await Db.Movies.AsNoTracking().SingleAsync(m => m.MovieId == stale.MovieId)
        ).Status.ShouldBe(MovieStatus.Ended);
        (
            await Db.Movies.AsNoTracking().SingleAsync(m => m.MovieId == upcoming.MovieId)
        ).Status.ShouldBe(MovieStatus.NowShowing);
        (
            await Db.Movies.AsNoTracking().SingleAsync(m => m.MovieId == neverScheduled.MovieId)
        ).Status.ShouldBe(MovieStatus.NowShowing);
        (
            await Db.Movies.AsNoTracking().SingleAsync(m => m.MovieId == comingSoon.MovieId)
        ).Status.ShouldBe(MovieStatus.ComingSoon);
    }
}

public class BookingServiceStopSellingTests(PostgresContainerFixture fixture) : BookingServiceTestBase(fixture), IClassFixture<PostgresContainerFixture>
{
    [Fact]
    public async Task HoldSeatsAsync_WithinStopSellingWindow_ThrowsBusinessRuleException()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(
            Db,
            movie,
            room,
            seats,
            DateTime.UtcNow.AddMinutes(20)
        );
        var seatIds = Db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .Select(ss => ss.ShowtimeSeatId)
            .ToList();
        var settings = TestDataSeeder.SetCinemaSettings(Db);
        settings.StopSellingMinutesBefore = 30;
        Db.SaveChanges();
        var sut = CreateHoldSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.HoldSeatsAsync(
                customer.UserId,
                new HoldSeatsRequest(
                    showtime.ShowtimeId,
                    "Nguyen Van A",
                    "0901234567",
                    seatIds,
                    null
                ),
                CancellationToken.None
            )
        );
    }
}

public class ShowtimeLifecycleUtilTests
{
    private static readonly DateTime Start = new(2026, 10, 28, 18, 0, 0);

    [Fact]
    public void IsOnlineSalesClosed_ZeroMinutes_NeverClosesEarly() =>
        ShowtimeLifecycle.IsOnlineSalesClosed(Start.AddMinutes(-1), Start, 0).ShouldBeFalse();

    [Fact]
    public void IsOnlineSalesClosed_BoundaryIsInclusive()
    {
        ShowtimeLifecycle.IsOnlineSalesClosed(Start.AddMinutes(-31), Start, 30).ShouldBeFalse();
        ShowtimeLifecycle.IsOnlineSalesClosed(Start.AddMinutes(-30), Start, 30).ShouldBeTrue();
    }
}
