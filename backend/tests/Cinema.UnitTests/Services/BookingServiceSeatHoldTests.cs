using Cinema.Application.DTOs.Bookings;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class BookingServiceSeatHoldTests : BookingServiceTestBase
{
    private (User EmployeeUser, Employee Employee, Showtime Showtime, List<ShowtimeSeat> Seats) SeedCounter(
        int seatCount,
        DateTime? startTime = null
    )
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: seatCount);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, startTime);
        var showtimeSeats = Db.ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId).ToList();
        return (employeeUser, employee, showtime, showtimeSeats);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_HoldLongerThanTimeToShowtime_ExpiresAtMostAtShowtimeStart()
    {
        var start = DateTime.UtcNow.AddMinutes(5);
        var (user, employee, showtime, seats) = SeedCounter(1, start);
        var sut = CreateSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, [seats[0].ShowtimeSeatId], null, null, "Khach", null),
            CancellationToken.None
        );

        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == response.BookingId);
        booking.ExpiresAt.ShouldNotBeNull();
        booking.ExpiresAt!.Value.ShouldBeLessThanOrEqualTo(showtime.StartTime);
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeSeatId == seats[0].ShowtimeSeatId);
        seat.HeldUntil!.Value.ShouldBeLessThanOrEqualTo(showtime.StartTime);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_BindsHeldSeatsToCreatedBooking()
    {
        var (user, employee, showtime, seats) = SeedCounter(2);
        var sut = CreateSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, seats.Select(s => s.ShowtimeSeatId).ToList(), null, null, "Khach", null),
            CancellationToken.None
        );

        var reloaded = await Db.ShowtimeSeats.AsNoTracking().Where(ss => ss.ShowtimeId == showtime.ShowtimeId).ToListAsync();
        reloaded.ShouldAllBe(ss => ss.HeldByBookingId == response.BookingId);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_SeatExpiredButHeldBySameHolder_CanBeHeldAgain()
    {
        var (user, employee, showtime, seats) = SeedCounter(1);
        var seat = seats[0];
        seat.Status = ShowtimeSeatStatus.Held;
        seat.HeldByUserId = user.UserId;
        seat.HeldUntil = DateTime.UtcNow.AddMinutes(-1);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, [seat.ShowtimeSeatId], null, null, "Khach", null),
            CancellationToken.None
        );

        var reloaded = await Db.ShowtimeSeats.AsNoTracking().SingleAsync(ss => ss.ShowtimeSeatId == seat.ShowtimeSeatId);
        reloaded.Status.ShouldBe(ShowtimeSeatStatus.Held);
        reloaded.HeldByBookingId.ShouldBe(response.BookingId);
        reloaded.HeldUntil!.Value.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_SeatExpiredButHeldByAnotherUser_ThrowsSeatConflict()
    {
        var (user, employee, showtime, seats) = SeedCounter(1);
        var other = TestDataSeeder.AddCustomer(Db, email: "other@test.local");
        var seat = seats[0];
        seat.Status = ShowtimeSeatStatus.Held;
        seat.HeldByUserId = other.UserId;
        seat.HeldUntil = DateTime.UtcNow.AddMinutes(-1);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<SeatConflictException>(() =>
            sut.CounterHoldSeatsAsync(
                employee.EmployeeId,
                user.UserId,
                new CounterHoldRequest(showtime.ShowtimeId, [seat.ShowtimeSeatId], null, null, "Khach", null),
                CancellationToken.None
            )
        );
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CounterCheckoutAsync_CounterBookingLinkedToMember_ConfirmsEmployeeHeldSeats(
        bool seatsBoundToBooking
    )
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var member = TestDataSeeder.AddCustomer(Db, email: "member@test.local");
        var (user, employee, showtime, seats) = SeedCounter(1);
        var sut = CreateSut();
        var hold = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, [seats[0].ShowtimeSeatId], null, member.UserId, "Khach", null),
            CancellationToken.None
        );
        if (!seatsBoundToBooking)
        {
            var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeSeatId == seats[0].ShowtimeSeatId);
            seat.HeldByBookingId = null;
            await Db.SaveChangesAsync();
        }

        sut = CreateSut();
        var result = await sut.CounterCheckoutAsync(
            hold.BookingId,
            new CounterCheckoutRequest(1_000_000m),
            CancellationToken.None
        );

        result.Status.ShouldBe("Confirmed");
        (await Db.BookingTickets.CountAsync(t => t.BookingId == hold.BookingId)).ShouldBe(1);
    }

    [Fact]
    public async Task CounterCheckoutAsync_OnlineBooking_ThrowsBusinessRuleExceptionAndCreatesNoPayment()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var (_, _, showtime, _) = SeedCounter(1);
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = "BK-ONLINE-0001",
            Channel = BookingChannel.Online,
            CustomerName = "Online",
            TotalAmount = 100_000m,
            FinalAmount = 100_000m,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        };
        Db.Bookings.Add(booking);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCheckoutAsync(booking.BookingId, new CounterCheckoutRequest(100_000m), CancellationToken.None)
        );

        (await Db.Payments.CountAsync(p => p.BookingId == booking.BookingId)).ShouldBe(0);
        (await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == booking.BookingId))
            .Status.ShouldBe(BookingStatus.Pending);
    }
}
