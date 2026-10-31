using Cinema.Application.DTOs.Bookings;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class BookingServiceCounterHoldAndCheckoutTests : BookingServiceTestBase
{
    [Fact]
    public async Task CounterHoldSeatsAsync_ValidRequest_CreatesCounterChannelBookingAttributedToEmployee()
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: 100_000m);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        var sut = CreateSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            employeeUser.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, [seatId], null, null, "Khach le", null),
            CancellationToken.None
        );

        response.Status.ShouldBe("Pending");
        response.Channel.ShouldBe("Counter");
        response.CreatedByEmployeeId.ShouldBe(employee.EmployeeId);
        response.FinalAmount.ShouldBe(100_000m);

        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == response.BookingId);
        booking.Channel.ShouldBe(BookingChannel.Counter);
        booking.CreatedByEmployeeId.ShouldBe(employee.EmployeeId);
    }

    private async Task<int> SeedCounterPendingBookingAsync(decimal basePrice = 100_000m)
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: basePrice);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        var sut = CreateSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId,
            employeeUser.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, [seatId], null, null, "Khach le", null),
            CancellationToken.None
        );
        return response.BookingId;
    }

    [Fact]
    public async Task CounterCheckoutAsync_AmountReceivedCoversTotal_ConfirmsBookingAndReturnsChange()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var bookingId = await SeedCounterPendingBookingAsync(basePrice: 100_000m);
        var sut = CreateSut();

        var result = await sut.CounterCheckoutAsync(
            bookingId,
            new CounterCheckoutRequest(150_000m),
            CancellationToken.None
        );

        result.Status.ShouldBe("Confirmed");
        result.Change.ShouldBe(50_000m);
        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == bookingId);
        booking.Status.ShouldBe(BookingStatus.Confirmed);
        var payment = await Db.Payments.SingleAsync(p => p.BookingId == bookingId);
        payment.PaymentMethod.ShouldBe(PaymentMethod.Cash);
        payment.Status.ShouldBe(PaymentStatus.Success);
        payment.AmountReceived.ShouldBe(150_000m);
        payment.ChangeGiven.ShouldBe(50_000m);
    }

    [Fact]
    public async Task CounterCheckoutAsync_AmountReceivedLessThanTotal_ThrowsBusinessRuleExceptionAndDoesNotCreatePayment()
    {
        var bookingId = await SeedCounterPendingBookingAsync(basePrice: 100_000m);
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCheckoutAsync(
                bookingId,
                new CounterCheckoutRequest(50_000m),
                CancellationToken.None
            )
        );

        (await Db.Payments.CountAsync(p => p.BookingId == bookingId)).ShouldBe(0);
        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == bookingId);
        booking.Status.ShouldBe(BookingStatus.Pending);
    }

    [Fact]
    public async Task CounterCheckoutAsync_BookingAlreadyExpired_ThrowsBusinessRuleException()
    {
        var bookingId = await SeedCounterPendingBookingAsync();
        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == bookingId);
        booking.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterCheckoutAsync(
                bookingId,
                new CounterCheckoutRequest(100_000m),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CounterCheckoutAsync_UnknownBookingId_ThrowsNotFound()
    {
        var sut = CreateSut();

        await Should.ThrowAsync<NotFoundException>(() =>
            sut.CounterCheckoutAsync(
                999,
                new CounterCheckoutRequest(100_000m),
                CancellationToken.None
            )
        );
    }
}

public class BookingServiceCheckinTests : BookingServiceTestBase
{
    private async Task<(string QrCode, int BookingId)> SeedConfirmedTicketAsync(
        DateTime? startTime = null
    )
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(
            Db,
            movie,
            room,
            seats,
            startTime: startTime ?? DateTime.UtcNow.AddMinutes(10)
        );
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;

        var holdSut = CreateSut();
        var hold = await holdSut.HoldSeatsAsync(
            customer.UserId,
            new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
            CancellationToken.None
        );
        var confirmSut = CreateSut();
        await confirmSut.ConfirmBookingAndGenerateTicketsAsync(
            hold.BookingId,
            CancellationToken.None
        );

        var ticket = await Db.BookingTickets.SingleAsync(t => t.BookingId == hold.BookingId);
        return (ticket.QrCode!, hold.BookingId);
    }

    [Fact]
    public async Task CheckinAsync_ValidQrWithinWindow_MarksTicketCheckedIn()
    {
        var (qrCode, bookingId) = await SeedConfirmedTicketAsync(
            startTime: DateTime.UtcNow.AddMinutes(10)
        );
        var sut = CreateSut();

        var result = await sut.CheckinAsync(new CheckinRequest(qrCode), CancellationToken.None);

        result.BookingId.ShouldBe(bookingId);
        var ticket = await Db.BookingTickets.SingleAsync(t => t.BookingId == bookingId);
        ticket.IsCheckedIn.ShouldBeTrue();
        ticket.CheckedInAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task CheckinAsync_UnknownQrCode_ThrowsNotFound()
    {
        var sut = CreateSut();

        await Should.ThrowAsync<NotFoundException>(() =>
            sut.CheckinAsync(new CheckinRequest("INVALID-QR"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task CheckinAsync_TicketAlreadyCheckedIn_ThrowsConflict()
    {
        var (qrCode, _) = await SeedConfirmedTicketAsync(startTime: DateTime.UtcNow.AddMinutes(10));
        var firstSut = CreateSut();
        await firstSut.CheckinAsync(new CheckinRequest(qrCode), CancellationToken.None);
        var secondSut = CreateSut();

        await Should.ThrowAsync<ConflictAppException>(() =>
            secondSut.CheckinAsync(new CheckinRequest(qrCode), CancellationToken.None)
        );
    }

    [Fact]
    public async Task CheckinAsync_MoreThan30MinutesBeforeShowtime_ThrowsBusinessRuleException()
    {
        var (qrCode, _) = await SeedConfirmedTicketAsync(startTime: DateTime.UtcNow.AddHours(2));
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CheckinAsync(new CheckinRequest(qrCode), CancellationToken.None)
        );
    }

    [Fact]
    public async Task CheckinAsync_ShowtimeAlreadyEnded_ThrowsBusinessRuleException()
    {
        var (qrCode, bookingId) = await SeedConfirmedTicketAsync(
            startTime: DateTime.UtcNow.AddMinutes(10)
        );
        var booking = await Db.Bookings.Include(b => b.Showtime).SingleAsync(b => b.BookingId == bookingId);
        booking.Showtime.StartTime = DateTime.UtcNow.AddHours(-5);
        booking.Showtime.EndTime = DateTime.UtcNow.AddHours(-3);
        await Db.SaveChangesAsync();

        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CheckinAsync(new CheckinRequest(qrCode), CancellationToken.None)
        );
    }
}

public class BookingServiceReverseLoyaltyTests : BookingServiceTestBase
{
    [Fact]
    public async Task ReverseLoyaltyAsync_CustomerHasEnoughPointsAndSpend_DeductsExactAmount()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var customer = TestDataSeeder.AddCustomer(Db);
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 50;
        profile.TotalSpent = 500_000m;
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.ReverseLoyaltyAsync(customer.UserId, 100_000m, CancellationToken.None);
        await Db.SaveChangesAsync();

        var reloaded = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        reloaded.Points.ShouldBe(40);
        reloaded.TotalSpent.ShouldBe(400_000m);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_ReversalWouldGoNegative_ClampsAtZeroInsteadOfGoingNegative()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var customer = TestDataSeeder.AddCustomer(Db);
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points = 2;
        profile.TotalSpent = 5_000m;
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await sut.ReverseLoyaltyAsync(customer.UserId, 100_000m, CancellationToken.None);
        await Db.SaveChangesAsync();

        var reloaded = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        reloaded.Points.ShouldBe(0);
        reloaded.TotalSpent.ShouldBe(0m);
    }

    [Fact]
    public async Task ReverseLoyaltyAsync_UserHasNoCustomerProfile_DoesNothingAndDoesNotThrow()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (employeeUser, _) = TestDataSeeder.AddEmployee(Db);
        var sut = CreateSut();

        await Should.NotThrowAsync(() =>
            sut.ReverseLoyaltyAsync(employeeUser.UserId, 100_000m, CancellationToken.None)
        );
    }

    // ── CounterHold + UserId tests (Task A) ──────────────────────────────────

    [Fact]
    public async Task CounterHoldSeatsAsync_WithUserId_UserNotFound_ThrowsNotFoundException()
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var seatId = (await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)).ShowtimeSeatId;
        var sut = CreateSut();

        await Should.ThrowAsync<NotFoundException>(() =>
            sut.CounterHoldSeatsAsync(
                employee.EmployeeId, employeeUser.UserId,
                new CounterHoldRequest(showtime.ShowtimeId, [seatId], null, 99999, "ignored", null),
                CancellationToken.None));
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_WithUserId_UserInactive_ThrowsBusinessRuleException()
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var customer = TestDataSeeder.AddCustomer(Db, email: "inactive@test.local", isActive: false);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var seatId = (await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)).ShowtimeSeatId;
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterHoldSeatsAsync(
                employee.EmployeeId, employeeUser.UserId,
                new CounterHoldRequest(showtime.ShowtimeId, [seatId], null, customer.UserId, "ignored", null),
                CancellationToken.None));
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_WithUserId_UserNotCustomerRole_ThrowsBusinessRuleException()
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var seatId = (await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)).ShowtimeSeatId;
        var sut = CreateSut();

        // employeeUser has AdminRoleId, not CustomerRoleId
        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CounterHoldSeatsAsync(
                employee.EmployeeId, employeeUser.UserId,
                new CounterHoldRequest(showtime.ShowtimeId, [seatId], null, employeeUser.UserId, "ignored", null),
                CancellationToken.None));
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_WithUserId_ValidCustomer_UsesUserNameAndPhone()
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var customer = TestDataSeeder.AddCustomer(Db, email: "linked@test.local");
        customer.FullName = "Nguyen Thi B";
        customer.Phone = "0912345678";
        await Db.SaveChangesAsync();
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var seatId = (await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)).ShowtimeSeatId;
        var sut = CreateSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId, employeeUser.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, [seatId], null, customer.UserId, "ignored-name", "ignored-phone"),
            CancellationToken.None);

        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == response.BookingId);
        booking.CustomerName.ShouldBe("Nguyen Thi B");
        booking.CustomerPhone.ShouldBe("0912345678");
        booking.UserId.ShouldBe(customer.UserId);
    }

    [Fact]
    public async Task CounterHoldSeatsAsync_WithoutUserId_UsesRequestNameAndPhone()
    {
        var (employeeUser, employee) = TestDataSeeder.AddEmployee(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var seatId = (await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)).ShowtimeSeatId;
        var sut = CreateSut();

        var response = await sut.CounterHoldSeatsAsync(
            employee.EmployeeId, employeeUser.UserId,
            new CounterHoldRequest(showtime.ShowtimeId, [seatId], null, null, "Khach Le", "0900000001"),
            CancellationToken.None);

        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == response.BookingId);
        booking.CustomerName.ShouldBe("Khach Le");
        booking.CustomerPhone.ShouldBe("0900000001");
        booking.UserId.ShouldBeNull();
    }
}
