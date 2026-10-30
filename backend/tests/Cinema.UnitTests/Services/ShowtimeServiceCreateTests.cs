using Cinema.Application.Utils;
using Cinema.Application.DTOs.Showtimes;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class ShowtimeServiceCreateTests : ShowtimeServiceTestBase
{
    private static DateTime SafeStartTime(int daysFromNow = 1) =>
        VietnamTime.ToUtc(VietnamTime.Today().AddDays(daysFromNow), new TimeOnly(14, 0));

    [Fact]
    public async Task CreateShowtimeAsync_ValidRequest_GeneratesOneShowtimeSeatPerActiveNonAisleSeat()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var activeSeats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 4);
        Db.Seats.Add(
            new Seat
            {
                RoomId = room.RoomId,
                SeatRow = "A",
                SeatNumber = 5,
                IsActive = false,
            }
        ); // inactive
        Db.Seats.Add(
            new Seat
            {
                RoomId = room.RoomId,
                SeatRow = "A",
                SeatNumber = 6,
                IsAisle = true,
            }
        );
        await Db.SaveChangesAsync();
        var movie = TestDataSeeder.AddMovie(
            Db,
            durationMinutes: 120,
            status: MovieStatus.NowShowing
        );
        var sut = CreateSut();

        var response = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(), 90_000m),
            CancellationToken.None
        );

        response.SeatsGenerated.ShouldBe(activeSeats.Count);
        response.Status.ShouldBe("Scheduled");
        (await Db.ShowtimeSeats.CountAsync(ss => ss.ShowtimeId == response.ShowtimeId)).ShouldBe(
            activeSeats.Count
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_SeatPriceUsesSeatTypeMultiplier()
    {
        var room = TestDataSeeder.AddRoom(Db);
        Db.Seats.Add(
            new Seat
            {
                RoomId = room.RoomId,
                SeatRow = "V",
                SeatNumber = 1,
                SeatTypeId = TestDataSeeder.VipSeatTypeId, // seeded PriceMultiplier 1.5
                IsActive = true,
            }
        );
        await Db.SaveChangesAsync();
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();

        var response = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(), 100_000m),
            CancellationToken.None
        );

        var vipShowtimeSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == response.ShowtimeId
        );
        vipShowtimeSeat.Price.ShouldBe(150_000m);
    }

    [Fact]
    public async Task CreateShowtimeAsync_MovieDoesNotExist_ThrowsValidationAppException()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var sut = CreateSut();

        await Should.ThrowAsync<ValidationAppException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(999, room.RoomId, SafeStartTime(), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_RoomDoesNotExist_ThrowsValidationAppException()
    {
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();

        await Should.ThrowAsync<ValidationAppException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, 999, SafeStartTime(), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_MovieEnded_ThrowsBusinessRuleException()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, status: MovieStatus.Ended);
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_RoomInactive_ThrowsBusinessRuleException()
    {
        var room = TestDataSeeder.AddRoom(Db, isActive: false);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_RoomHasNoSeatMap_ThrowsBusinessRuleException()
    {
        var room = TestDataSeeder.AddRoom(Db); // no seats added
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_OutsideOpeningHours_ThrowsBusinessRuleException()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();
        var tooEarly = VietnamTime.ToUtc(VietnamTime.Today().AddDays(1), new TimeOnly(6, 0));

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, tooEarly, 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_OverlappingTimeSlotInSameRoom_ThrowsConflict()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 120); // + 15min buffer = 135min block
        var sut = CreateSut();
        var firstStart = SafeStartTime();
        await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, firstStart, 90_000m),
            CancellationToken.None
        );

        // Starts 1h after the first showtime's start — well before it ends (135 min later).
        var overlappingStart = firstStart.AddHours(1);

        await Should.ThrowAsync<ConflictAppException>(() =>
            sut.CreateShowtimeAsync(
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, overlappingStart, 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CreateShowtimeAsync_BackToBackAfterBufferInSameRoom_Succeeds()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 100);
        RoomBufferMinutes = 20;
        var sut = CreateSut();
        var firstStart = SafeStartTime();
        var first = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, firstStart, 90_000m),
            CancellationToken.None
        );

        // First showtime ends exactly at first.EndTime (100min movie + 20min buffer) — starting
        // the next showtime right at that boundary must not be treated as an overlap.
        var second = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, first.EndTime, 90_000m),
            CancellationToken.None
        );

        second.ShowtimeId.ShouldNotBe(first.ShowtimeId);
    }

    [Fact]
    public async Task CreateShowtimeAsync_OverlapCheckIgnoresCancelledShowtimesInSameRoom()
    {
        var room = TestDataSeeder.AddRoom(Db);
        TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db, durationMinutes: 120);
        var sut = CreateSut();
        var start = SafeStartTime();
        var first = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, start, 90_000m),
            CancellationToken.None
        );
        await sut.DeleteShowtimeAsync(first.ShowtimeId, CancellationToken.None);

        // Same slot should now be free again since the earlier showtime was cancelled.
        var second = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, start, 90_000m),
            CancellationToken.None
        );

        second.ShowtimeId.ShouldNotBe(first.ShowtimeId);
    }
}

public class ShowtimeServiceUpdateAndDeleteTests : ShowtimeServiceTestBase
{
    private static DateTime SafeStartTime(int daysFromNow = 1) =>
        VietnamTime.ToUtc(VietnamTime.Today().AddDays(daysFromNow), new TimeOnly(14, 0));

    [Fact]
    public async Task UpdateShowtimeAsync_ChangingRoomId_ThrowsBusinessRuleException()
    {
        var room1 = TestDataSeeder.AddRoom(Db, name: "Room 1");
        var room2 = TestDataSeeder.AddRoom(Db, name: "Room 2");
        TestDataSeeder.AddSeats(Db, room1.RoomId, count: 1);
        TestDataSeeder.AddSeats(Db, room2.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();
        var created = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room1.RoomId, SafeStartTime(), 90_000m),
            CancellationToken.None
        );

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.UpdateShowtimeAsync(
                created.ShowtimeId,
                new UpsertShowtimeRequest(movie.MovieId, room2.RoomId, SafeStartTime(2), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task UpdateShowtimeAsync_HasConfirmedBooking_ThrowsConflict()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtimeSut = CreateSut();
        var created = await showtimeSut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(), 90_000m),
            CancellationToken.None
        );
        Db.Bookings.Add(
            new Booking
            {
                ShowtimeId = created.ShowtimeId,
                BookingCode = "BK-CONFIRMED-1",
                CustomerName = "Nguyen Van A",
                Status = BookingStatus.Confirmed,
                TotalAmount = 90_000m,
                FinalAmount = 90_000m,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();

        await Should.ThrowAsync<ConflictAppException>(() =>
            showtimeSut.UpdateShowtimeAsync(
                created.ShowtimeId,
                new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(3), 90_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task DeleteShowtimeAsync_CancelsShowtimeAndPendingBookingsAndReleasesHeldSeats()
    {
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var sut = CreateSut();
        var created = await sut.CreateShowtimeAsync(
            new UpsertShowtimeRequest(movie.MovieId, room.RoomId, SafeStartTime(), 90_000m),
            CancellationToken.None
        );

        var showtimeSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == created.ShowtimeId
        );
        showtimeSeat.Status = ShowtimeSeatStatus.Held;
        showtimeSeat.HeldByUserId = TestDataSeeder.AddCustomer(Db).UserId;
        showtimeSeat.HeldUntil = DateTime.UtcNow.AddMinutes(10);
        Db.Bookings.Add(
            new Booking
            {
                ShowtimeId = created.ShowtimeId,
                BookingCode = "BK-PENDING-1",
                CustomerName = "Nguyen Van A",
                Status = BookingStatus.Pending,
                TotalAmount = 90_000m,
                FinalAmount = 90_000m,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            }
        );
        await Db.SaveChangesAsync();

        // The manual mutations above were tracked directly on this context (same `sut`,
        // no intervening CreateSut() call) — DeleteShowtimeAsync's bulk ExecuteUpdateAsync
        // calls bypass the tracker, so without clearing it here the entities we just
        // touched would keep resolving to their pre-delete in-memory values afterwards.
        Db.ChangeTracker.Clear();

        var result = await sut.DeleteShowtimeAsync(created.ShowtimeId, CancellationToken.None);

        result.Status.ShouldBe("Cancelled");
        var reloadedShowtime = await Db.Showtimes.SingleAsync(s =>
            s.ShowtimeId == created.ShowtimeId
        );
        reloadedShowtime.Status.ShouldBe(ShowtimeStatus.Cancelled);
        var reloadedBooking = await Db.Bookings.SingleAsync(b =>
            b.ShowtimeId == created.ShowtimeId
        );
        reloadedBooking.Status.ShouldBe(BookingStatus.Cancelled);
        var reloadedSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == created.ShowtimeId
        );
        reloadedSeat.Status.ShouldBe(ShowtimeSeatStatus.Available);
        reloadedSeat.HeldByUserId.ShouldBeNull();
    }
}
