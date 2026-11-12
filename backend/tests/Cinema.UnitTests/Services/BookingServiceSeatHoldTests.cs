using Cinema.Application.DTOs.Bookings;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class BookingServiceSeatHoldTests(PostgresContainerFixture fixture) : BookingServiceTestBase(fixture)
{
    private (
        User EmployeeUser,
        Employee Employee,
        Showtime Showtime,
        List<ShowtimeSeat> Seats
    ) SeedCounter(int seatCount, DateTime? startTime = null)
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: seatCount);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, startTime);
        var showtimeSeats = Db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .ToList();
        return (employeeUser, employee, showtime, showtimeSeats);
    }

    private static CounterHoldRequest HoldRequest(Showtime showtime, ShowtimeSeat seat) =>
        new(showtime.ShowtimeId, [seat.ShowtimeSeatId], null, null, "Khach", null);

    [Fact]
    public async Task CounterHoldSeatsAsync_HoldLongerThanTimeToShowtime_ExpiresAtMostAtShowtimeStart()
    {
        var start = DateTime.UtcNow.AddMinutes(5);
        var (user, employee, showtime, seats) = SeedCounter(1, start);
        var sut = CreateCounterSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(
                showtime.ShowtimeId,
                [seats[0].ShowtimeSeatId],
                null,
                null,
                "Khach",
                null
            ),
            CancellationToken.None
        );

        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == response.BookingId);
        booking.ExpiresAt.ShouldNotBeNull();
        booking.ExpiresAt!.Value.ShouldBeLessThanOrEqualTo(showtime.StartTime);
        var seat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeSeatId == seats[0].ShowtimeSeatId
        );
        seat.HeldUntil!.Value.ShouldBeLessThanOrEqualTo(showtime.StartTime);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_BindsHeldSeatsToCreatedBooking()
    {
        var (user, employee, showtime, seats) = SeedCounter(2);
        var sut = CreateCounterSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(
                showtime.ShowtimeId,
                seats.Select(s => s.ShowtimeSeatId).ToList(),
                null,
                null,
                "Khach",
                null
            ),
            CancellationToken.None
        );

        var reloaded = await Db
            .ShowtimeSeats.AsNoTracking()
            .Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .ToListAsync();
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
        var sut = CreateCounterSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(
                showtime.ShowtimeId,
                [seat.ShowtimeSeatId],
                null,
                null,
                "Khach",
                null
            ),
            CancellationToken.None
        );

        var reloaded = await Db
            .ShowtimeSeats.AsNoTracking()
            .SingleAsync(ss => ss.ShowtimeSeatId == seat.ShowtimeSeatId);
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
        var sut = CreateCounterSut();

        await Should.ThrowAsync<SeatConflictException>(() =>
            sut.CounterHoldSeatsAsync(
                employee.EmployeeId,
                user.UserId,
                new CounterHoldRequest(
                    showtime.ShowtimeId,
                    [seat.ShowtimeSeatId],
                    null,
                    null,
                    "Khach",
                    null
                ),
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
        var sut = CreateCounterSut();
        var hold = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            new CounterHoldRequest(
                showtime.ShowtimeId,
                [seats[0].ShowtimeSeatId],
                null,
                member.UserId,
                "Khach",
                null
            ),
            CancellationToken.None
        );
        if (!seatsBoundToBooking)
        {
            var seat = await Db.ShowtimeSeats.SingleAsync(ss =>
                ss.ShowtimeSeatId == seats[0].ShowtimeSeatId
            );
            seat.HeldByBookingId = null;
            await Db.SaveChangesAsync();
        }

        sut = CreateCounterSut();
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
        var sut = CreateCounterSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCheckoutAsync(
                booking.BookingId,
                new CounterCheckoutRequest(100_000m),
                CancellationToken.None
            )
        );

        (await Db.Payments.CountAsync(p => p.BookingId == booking.BookingId)).ShouldBe(0);
        (
            await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == booking.BookingId)
        ).Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_SecondHoldOnSameShowtimeBySameEmployee_Succeeds()
    {
        var (user, employee, showtime, seats) = SeedCounter(2);
        var sut = CreateCounterSut();
        await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[0]),
            CancellationToken.None
        );

        var second = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[1]),
            CancellationToken.None
        );

        second.Status.ShouldBe("Pending");
        (
            await Db.Bookings.CountAsync(b =>
                b.Channel == BookingChannel.Counter && b.Status == BookingStatus.Pending
            )
        ).ShouldBe(2);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_SixthPendingCounterBookingOfSameEmployee_ThrowsConflict()
    {
        var (user, employee, showtime, seats) = SeedCounter(6);
        var sut = CreateCounterSut();
        for (var i = 0; i < 5; i++)
            await sut.CounterHoldSeatsAsync(
                employee.EmployeeId,
                user.UserId,
                HoldRequest(showtime, seats[i]),
                CancellationToken.None
            );

        var ex = await Should.ThrowAsync<ConflictAppException>(() =>
            sut.CounterHoldSeatsAsync(
                employee.EmployeeId,
                user.UserId,
                HoldRequest(showtime, seats[5]),
                CancellationToken.None
            )
        );

        ex.Message.ShouldBe(Messages.Rule_CounterPendingLimit);
        (
            await Db
                .ShowtimeSeats.AsNoTracking()
                .SingleAsync(ss => ss.ShowtimeSeatId == seats[5].ShowtimeSeatId)
        ).Status.ShouldBe(ShowtimeSeatStatus.Available);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_CancellingOnePendingFreesSlotUnderLimit()
    {
        var (user, employee, showtime, seats) = SeedCounter(6);
        var sut = CreateCounterSut();
        var first = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[0]),
            CancellationToken.None
        );
        for (var i = 1; i < 5; i++)
            await sut.CounterHoldSeatsAsync(
                employee.EmployeeId,
                user.UserId,
                HoldRequest(showtime, seats[i]),
                CancellationToken.None
            );
        await sut.CounterCancelAsync(first.BookingId, employee.EmployeeId, CancellationToken.None);

        var next = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[5]),
            CancellationToken.None
        );

        next.Status.ShouldBe("Pending");
    }

    [Fact]
    public async Task CounterCancelAsync_PendingCounterBooking_CancelsAndReleasesSeats()
    {
        var (user, employee, showtime, seats) = SeedCounter(1);
        var sut = CreateCounterSut();
        var hold = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[0]),
            CancellationToken.None
        );
        var (_, otherEmployee) = TestDataSeeder.AddEmployee(Db, email: "other-employee@test.local");

        var result = await sut.CounterCancelAsync(
            hold.BookingId,
            otherEmployee.EmployeeId,
            CancellationToken.None
        );

        result.BookingId.ShouldBe(hold.BookingId);
        result.Status.ShouldBe(BookingStatus.Cancelled.ToDtoStatus());
        result.RefundAmount.ShouldBeNull();
        (
            await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == hold.BookingId)
        ).Status.ShouldBe(BookingStatus.Cancelled);
        var seat = await Db
            .ShowtimeSeats.AsNoTracking()
            .SingleAsync(ss => ss.ShowtimeSeatId == seats[0].ShowtimeSeatId);
        seat.Status.ShouldBe(ShowtimeSeatStatus.Available);
        seat.HeldByUserId.ShouldBeNull();
        seat.HeldByBookingId.ShouldBeNull();
        RealtimeNotifierMock.Verify(
            n =>
                n.NotifySeatReleasedAsync(
                    showtime.ShowtimeId,
                    It.IsAny<List<int>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task CounterCancelAsync_AlreadyCancelled_IsIdempotent()
    {
        var (user, employee, showtime, seats) = SeedCounter(1);
        var sut = CreateCounterSut();
        var hold = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[0]),
            CancellationToken.None
        );
        await sut.CounterCancelAsync(hold.BookingId, employee.EmployeeId, CancellationToken.None);

        var again = await sut.CounterCancelAsync(
            hold.BookingId,
            employee.EmployeeId,
            CancellationToken.None
        );

        again.Status.ShouldBe(BookingStatus.Cancelled.ToDtoStatus());
        RealtimeNotifierMock.Verify(
            n =>
                n.NotifySeatReleasedAsync(
                    It.IsAny<int>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task CounterCancelAsync_ConfirmedCounterBooking_ThrowsBusinessRule()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (user, employee, showtime, seats) = SeedCounter(1);
        var sut = CreateCounterSut();
        var hold = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[0]),
            CancellationToken.None
        );
        await sut.CounterCheckoutAsync(
            hold.BookingId,
            new CounterCheckoutRequest(1_000_000m),
            CancellationToken.None
        );

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCancelAsync(hold.BookingId, employee.EmployeeId, CancellationToken.None)
        );

        ex.Message.ShouldBe(Messages.Rule_OnlyCounterPendingCancellable);
        (
            await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == hold.BookingId)
        ).Status.ShouldBe(BookingStatus.Confirmed);
    }

    [Fact]
    public async Task CounterCancelAsync_OnlineBooking_ThrowsBusinessRuleAndKeepsPending()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var (_, employee, showtime, _) = SeedCounter(1);
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = "BK-ONLINE-0002",
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
        var sut = CreateCounterSut();

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCancelAsync(booking.BookingId, employee.EmployeeId, CancellationToken.None)
        );

        ex.Message.ShouldBe(Messages.Rule_OnlyCounterPendingCancellable);
        (
            await Db.Bookings.AsNoTracking().SingleAsync(b => b.BookingId == booking.BookingId)
        ).Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public async Task CounterCancelAsync_UnknownBooking_ThrowsNotFound()
    {
        var (_, employee, _, _) = SeedCounter(1);
        var sut = CreateCounterSut();

        await Should.ThrowAsync<NotFoundException>(() =>
            sut.CounterCancelAsync(999_999, employee.EmployeeId, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CounterCheckoutAsync_AfterCancel_ThrowsBusinessRuleAndCreatesNoPayment()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (user, employee, showtime, seats) = SeedCounter(1);
        var sut = CreateCounterSut();
        var hold = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            user.UserId,
            HoldRequest(showtime, seats[0]),
            CancellationToken.None
        );
        await sut.CounterCancelAsync(hold.BookingId, employee.EmployeeId, CancellationToken.None);

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCheckoutAsync(
                hold.BookingId,
                new CounterCheckoutRequest(1_000_000m),
                CancellationToken.None
            )
        );

        ex.Message.ShouldBe(Messages.Rule_BookingNotPendingOrHoldExpired);
        (await Db.Payments.CountAsync(p => p.BookingId == hold.BookingId)).ShouldBe(0);
    }
}
