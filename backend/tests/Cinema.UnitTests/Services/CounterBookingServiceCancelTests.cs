using Cinema.Application.DTOs.Bookings;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class CounterBookingServiceCancelTests : BookingServiceTestBase
{
    private async Task<(Booking Booking, User Customer, Showtime Showtime)> SeedHeldBookingAsync(
        decimal basePrice = 100_000m,
        DateTime? startTime = null
    )
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
            basePrice: basePrice,
            startTime: startTime
        );
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;

        var holdingSut = CreateHoldSut();
        var hold = await holdingSut.HoldSeatsAsync(
            customer.UserId,
            new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
            CancellationToken.None
        );
        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == hold.BookingId);
        return (booking, customer, showtime);
    }

    [Fact]
    public async Task CounterCancelAsync_OnlinePendingBookingWithHeldSeats_ThrowsAndKeepsSeatsHeld()
    {
        var (booking, _, _) = await SeedHeldBookingAsync();
        var (_, employee) = TestDataSeeder.AddEmployee(Db);
        var sut = CreateCounterSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCancelAsync(booking.BookingId, employee.EmployeeId, CancellationToken.None)
        );

        (
            await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == booking.BookingId)
        ).Status.ShouldBe(BookingStatus.Pending);
        (
            await Db
                .ShowtimeSeats.AsNoTracking()
                .SingleAsync(ss => ss.ShowtimeId == booking.ShowtimeId)
        ).Status.ShouldBe(ShowtimeSeatStatus.Held);
    }
}
