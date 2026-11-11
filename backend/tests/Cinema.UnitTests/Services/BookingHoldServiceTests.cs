using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class BookingHoldServiceTests : BookingServiceTestBase
{
    [Fact]
    public async Task HoldSeatsAsync_AvailableSeats_CreatesPendingBookingAndMarksSeatsHeld()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: 100_000m);
        var seatIds = Db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .Select(ss => ss.ShowtimeSeatId)
            .ToList();
        var sut = CreateHoldSut();

        var request = new HoldSeatsRequest(
            showtime.ShowtimeId,
            "Nguyen Van A",
            "0901234567",
            seatIds,
            null
        );
        var response = await sut.HoldSeatsAsync(customer.UserId, request, CancellationToken.None);

        response.Status.ShouldBe("Pending");
        response.TotalAmount.ShouldBe(200_000m);
        response.FinalAmount.ShouldBe(200_000m);

        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == response.BookingId);
        booking.Status.ShouldBe(BookingStatus.Pending);
        booking.UserId.ShouldBe(customer.UserId);
        booking.ExpiresAt.ShouldNotBeNull();

        var heldSeats = await Db
            .ShowtimeSeats.Where(ss => seatIds.Contains(ss.ShowtimeSeatId))
            .ToListAsync();
        heldSeats.ShouldAllBe(s =>
            s.Status == ShowtimeSeatStatus.Held && s.HeldByUserId == customer.UserId
        );
    }

    [Fact]
    public async Task HoldSeatsAsync_OneSeatAlreadyHeldByAnotherUser_ThrowsSeatConflictAndRollsBackOtherHolds()
    {
        var customer = TestDataSeeder.AddCustomer(Db, email: "a@test.local");
        var otherUserId = TestDataSeeder.AddCustomer(Db, email: "b@test.local").UserId;
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var showtimeSeats = await Db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .OrderBy(ss => ss.ShowtimeSeatId)
            .ToListAsync();

        showtimeSeats[1].Status = ShowtimeSeatStatus.Held;
        showtimeSeats[1].HeldByUserId = otherUserId;
        showtimeSeats[1].HeldUntil = DateTime.UtcNow.AddMinutes(10);
        await Db.SaveChangesAsync();

        var sut = CreateHoldSut();
        var seatIds = showtimeSeats.Select(s => s.ShowtimeSeatId).ToList();
        var request = new HoldSeatsRequest(
            showtime.ShowtimeId,
            "Nguyen Van A",
            null,
            seatIds,
            null
        );

        var ex = await Should.ThrowAsync<SeatConflictException>(() =>
            sut.HoldSeatsAsync(customer.UserId, request, CancellationToken.None)
        );
        ex.ConflictSeatIds.ShouldBe([showtimeSeats[1].ShowtimeSeatId]);

        var seat1AfterRollback = await Db.ShowtimeSeats.SingleAsync(s =>
            s.ShowtimeSeatId == showtimeSeats[0].ShowtimeSeatId
        );
        seat1AfterRollback.Status.ShouldBe(ShowtimeSeatStatus.Available);
        seat1AfterRollback.HeldByUserId.ShouldBeNull();

        (await Db.Bookings.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task HoldSeatsAsync_ShowtimeDoesNotExist_ThrowsNotFound()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var sut = CreateHoldSut();
        var request = new HoldSeatsRequest(999, "Nguyen Van A", null, [1], null);

        await Should.ThrowAsync<NotFoundException>(() =>
            sut.HoldSeatsAsync(customer.UserId, request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HoldSeatsAsync_ShowtimeNotScheduled_ThrowsBusinessRuleException()
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
            status: ShowtimeStatus.Completed
        );
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        var sut = CreateHoldSut();

        var request = new HoldSeatsRequest(
            showtime.ShowtimeId,
            "Nguyen Van A",
            null,
            [seatId],
            null
        );

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.HoldSeatsAsync(customer.UserId, request, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HoldSeatsAsync_CustomerAlreadyHasActivePendingBookingForShowtime_ThrowsConflict()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 4);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var seatIds = await Db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .Select(ss => ss.ShowtimeSeatId)
            .ToListAsync();
        var sut = CreateHoldSut();
        var firstRequest = new HoldSeatsRequest(
            showtime.ShowtimeId,
            "Nguyen Van A",
            null,
            [seatIds[0]],
            null
        );
        await sut.HoldSeatsAsync(customer.UserId, firstRequest, CancellationToken.None);

        var secondRequest = new HoldSeatsRequest(
            showtime.ShowtimeId,
            "Nguyen Van A",
            null,
            [seatIds[1]],
            null
        );

        await Should.ThrowAsync<ConflictAppException>(() =>
            sut.HoldSeatsAsync(customer.UserId, secondRequest, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HoldSeatsAsync_CustomerWithDiscountTier_AppliesDiscountToFinalAmount()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        await LoyaltyTestHelper.ReplaceTiersAsync(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 200, DateTime.UtcNow);
        await Db.SaveChangesAsync();

        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: 100_000m);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        var sut = CreateHoldSut();

        var response = await sut.HoldSeatsAsync(
            customer.UserId,
            new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
            CancellationToken.None
        );

        response.TotalAmount.ShouldBe(100_000m);
        response.DiscountAmount.ShouldBe(10_000m);
        response.FinalAmount.ShouldBe(90_000m);
    }
}
