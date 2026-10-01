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

public class BookingServiceHoldSeatsTests : BookingServiceTestBase
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
        var sut = CreateSut();

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

        var sut = CreateSut();
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
        var sut = CreateSut();
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
        var sut = CreateSut();

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
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 2);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats);
        var seatIds = await Db
            .ShowtimeSeats.Where(ss => ss.ShowtimeId == showtime.ShowtimeId)
            .Select(ss => ss.ShowtimeSeatId)
            .ToListAsync();
        var sut = CreateSut();
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
        var tier = new MembershipTier
        {
            TierName = "Gold",
            MinPoints = 0,
            DiscountPercent = 10m,
        };
        Db.MembershipTiers.Add(tier);
        await Db.SaveChangesAsync();
        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.TierId = tier.TierId;
        await Db.SaveChangesAsync();

        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: 100_000m);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;
        var sut = CreateSut();

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

public class BookingServiceCheckoutTests : BookingServiceTestBase
{
    private async Task<(Booking Booking, User Customer)> SeedPendingBookingAsync(
        decimal basePrice = 100_000m
    )
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        var room = TestDataSeeder.AddRoom(Db);
        var seats = TestDataSeeder.AddSeats(Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(Db);
        var showtime = TestDataSeeder.AddShowtime(Db, movie, room, seats, basePrice: basePrice);
        var seatId = (
            await Db.ShowtimeSeats.FirstAsync(ss => ss.ShowtimeId == showtime.ShowtimeId)
        ).ShowtimeSeatId;

        var holdingSut = CreateSut();
        var hold = await holdingSut.HoldSeatsAsync(
            customer.UserId,
            new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
            CancellationToken.None
        );
        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == hold.BookingId);
        return (booking, customer);
    }

    [Fact]
    public async Task CheckoutAsync_PendingBookingWithConfiguredGateway_CreatesPendingPaymentAndReturnsPaymentUrl()
    {
        var (booking, customer) = await SeedPendingBookingAsync();
        var gatewayMock = AddPaymentGateway("VNPay");
        gatewayMock
            .Setup(g =>
                g.CreatePaymentAsync(
                    It.IsAny<CreatePaymentRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new CreatePaymentResult("https://sandbox.vnpayment.vn/pay?x=1", "TXN123")
            );
        var sut = CreateSut();

        var result = await sut.CheckoutAsync(
            customer.UserId,
            booking.BookingId,
            new CheckoutRequest("VNPay"),
            CancellationToken.None
        );

        result.PaymentUrl.ShouldBe("https://sandbox.vnpayment.vn/pay?x=1");
        var payment = await Db.Payments.SingleAsync(p => p.BookingId == booking.BookingId);
        payment.Status.ShouldBe(PaymentStatus.Pending);
        payment.Amount.ShouldBe(booking.FinalAmount);
        payment.PaymentMethod.ShouldBe(PaymentMethod.VNPay);
    }

    [Fact]
    public async Task CheckoutAsync_UnconfiguredGateway_ThrowsBusinessRuleException()
    {
        var (booking, customer) = await SeedPendingBookingAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CheckoutAsync(
                customer.UserId,
                booking.BookingId,
                new CheckoutRequest("VNPay"),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CheckoutAsync_NotBookingOwner_ThrowsForbidden()
    {
        var (booking, _) = await SeedPendingBookingAsync();
        var stranger = TestDataSeeder.AddCustomer(Db, email: "stranger@test.local");
        AddPaymentGateway("VNPay");
        var sut = CreateSut();

        await Should.ThrowAsync<ForbiddenAppException>(() =>
            sut.CheckoutAsync(
                stranger.UserId,
                booking.BookingId,
                new CheckoutRequest("VNPay"),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CheckoutAsync_BookingAlreadyExpired_ThrowsBusinessRuleException()
    {
        var (booking, customer) = await SeedPendingBookingAsync();
        booking.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);
        await Db.SaveChangesAsync();
        AddPaymentGateway("VNPay");
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CheckoutAsync(
                customer.UserId,
                booking.BookingId,
                new CheckoutRequest("VNPay"),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task CheckoutAsync_BookingNotPending_ThrowsBusinessRuleException()
    {
        var (booking, customer) = await SeedPendingBookingAsync();
        booking.Status = BookingStatus.Cancelled;
        await Db.SaveChangesAsync();
        AddPaymentGateway("VNPay");
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CheckoutAsync(
                customer.UserId,
                booking.BookingId,
                new CheckoutRequest("VNPay"),
                CancellationToken.None
            )
        );
    }
}

public class BookingServiceConfirmAndCancelTests : BookingServiceTestBase
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

        var holdingSut = CreateSut();
        var hold = await holdingSut.HoldSeatsAsync(
            customer.UserId,
            new HoldSeatsRequest(showtime.ShowtimeId, "Nguyen Van A", null, [seatId], null),
            CancellationToken.None
        );
        var booking = await Db.Bookings.SingleAsync(b => b.BookingId == hold.BookingId);
        return (booking, customer, showtime);
    }

    [Fact]
    public async Task ConfirmBookingAndGenerateTicketsAsync_HeldSeatsStillValid_BooksSeatsAndGeneratesTicketsAndLoyaltyPoints()
    {
        TestDataSeeder.SetCinemaSettings(Db, pointsPerAmount: 10_000m);
        var (booking, customer, _) = await SeedHeldBookingAsync(basePrice: 100_000m);
        var sut = CreateSut();

        await sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None);

        var confirmed = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        confirmed.Status.ShouldBe(BookingStatus.Confirmed);

        var tickets = await Db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .ToListAsync();
        tickets.Count.ShouldBe(1);
        tickets[0].QrCode.ShouldNotBeNullOrWhiteSpace();

        var bookedSeat = await Db.ShowtimeSeats.SingleAsync(ss =>
            ss.ShowtimeId == booking.ShowtimeId
        );
        bookedSeat.Status.ShouldBe(ShowtimeSeatStatus.Booked);

        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points.ShouldBe(10); 
        profile.TotalSpent.ShouldBe(100_000m);
    }

    [Fact]
    public async Task ConfirmBookingAndGenerateTicketsAsync_AlreadyConfirmed_IsIdempotentAndDoesNotDuplicateTickets()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (booking, _, _) = await SeedHeldBookingAsync();
        var sut = CreateSut();
        await sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None);

        await sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None);

        var tickets = await Db
            .BookingTickets.Where(t => t.BookingId == booking.BookingId)
            .ToListAsync();
        tickets.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ConfirmBookingAndGenerateTicketsAsync_SeatsNoLongerHeld_CancelsBookingAndThrows()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (booking, _, _) = await SeedHeldBookingAsync();
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == booking.ShowtimeId);
        seat.Status = ShowtimeSeatStatus.Available;
        seat.HeldByUserId = null;
        await Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<SeatsNoLongerHeldException>(() =>
            sut.ConfirmBookingAndGenerateTicketsAsync(booking.BookingId, CancellationToken.None)
        );

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Cancelled);
    }

    [Fact]
    public async Task CancelBookingAsync_PendingBooking_ReleasesSeatsAndCancelsWithoutRefund()
    {
        var (booking, customer, _) = await SeedHeldBookingAsync();
        var sut = CreateSut();

        var result = await sut.CancelBookingAsync(
            customer.UserId,
            booking.BookingId,
            CancellationToken.None
        );

        result.Status.ShouldBe("Cancelled");
        result.RefundAmount.ShouldBeNull();
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == booking.ShowtimeId);
        seat.Status.ShouldBe(ShowtimeSeatStatus.Available);
    }

    [Fact]
    public async Task CancelBookingAsync_ConfirmedBookingWithinRefundWindow_RefundsAndCancels()
    {
        TestDataSeeder.SetCinemaSettings(
            Db,
            cancelRefundPercent: 100m,
            cancelDeadlineHours: 2,
            pointsPerAmount: 10_000m
        );
        var (booking, customer, _) = await SeedHeldBookingAsync(
            startTime: DateTime.UtcNow.AddDays(2)
        );
        var confirmSut = CreateSut();
        await confirmSut.ConfirmBookingAndGenerateTicketsAsync(
            booking.BookingId,
            CancellationToken.None
        );

        var payment = new Payment
        {
            BookingId = booking.BookingId,
            PaymentMethod = PaymentMethod.VNPay,
            Status = PaymentStatus.Success,
            Amount = booking.FinalAmount,
            TransactionId = "TXN-1",
            PaidAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };
        Db.Payments.Add(payment);
        await Db.SaveChangesAsync();

        var gatewayMock = AddPaymentGateway("VNPay");
        gatewayMock
            .Setup(g => g.RefundAsync(It.IsAny<RefundRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RefundResult(true, "REFUND-1", null));
        var sut = CreateSut();

        var result = await sut.CancelBookingAsync(
            customer.UserId,
            booking.BookingId,
            CancellationToken.None
        );

        result.Status.ShouldBe("Cancelled");
        result.RefundAmount.ShouldBe(booking.FinalAmount);
        result.RefundStatus.ShouldBe("Refunded");

        var reloadedPayment = await Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloadedPayment.Status.ShouldBe(PaymentStatus.Refunded);

        var profile = await Db.CustomerProfiles.SingleAsync(p => p.UserId == customer.UserId);
        profile.Points.ShouldBe(0); 
        profile.TotalSpent.ShouldBe(0m);
    }

    [Fact]
    public async Task CancelBookingAsync_ConfirmedBookingPastRefundDeadline_ThrowsBusinessRuleException()
    {
        TestDataSeeder.SetCinemaSettings(Db, cancelRefundPercent: 100m, cancelDeadlineHours: 2);
        var (booking, customer, _) = await SeedHeldBookingAsync(
            startTime: DateTime.UtcNow.AddHours(1)
        );
        var confirmSut = CreateSut();
        await confirmSut.ConfirmBookingAndGenerateTicketsAsync(
            booking.BookingId,
            CancellationToken.None
        );
        Db.Payments.Add(
            new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethod = PaymentMethod.VNPay,
                Status = PaymentStatus.Success,
                Amount = booking.FinalAmount,
                TransactionId = "TXN-1",
                PaidAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            }
        );
        await Db.SaveChangesAsync();
        AddPaymentGateway("VNPay");
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CancelBookingAsync(customer.UserId, booking.BookingId, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CalculateRefundEligibilityAsync_ZeroRefundPercent_NotEligible()
    {
        TestDataSeeder.SetCinemaSettings(Db, cancelRefundPercent: 0m);
        var sut = CreateSut();

        var result = await sut.CalculateRefundEligibilityAsync(
            DateTime.UtcNow.AddDays(2),
            100_000m,
            CancellationToken.None
        );

        result.Eligible.ShouldBeFalse();
        result.RefundAmount.ShouldBe(0m);
    }

    [Fact]
    public async Task CalculateRefundEligibilityAsync_WithinDeadlineWindow_ReturnsRoundedRefundAmount()
    {
        TestDataSeeder.SetCinemaSettings(Db, cancelRefundPercent: 90m, cancelDeadlineHours: 2);
        var sut = CreateSut();

        var result = await sut.CalculateRefundEligibilityAsync(
            DateTime.UtcNow.AddDays(2),
            100_000m,
            CancellationToken.None
        );

        result.Eligible.ShouldBeTrue();
        result.RefundAmount.ShouldBe(90_000m);
    }

    [Fact]
    public async Task ProcessPaymentFailureAsync_PendingBooking_ReleasesSeatsAndCancelsBooking()
    {
        var (booking, _, _) = await SeedHeldBookingAsync();
        var sut = CreateSut();

        await sut.ProcessPaymentFailureAsync(booking.BookingId, CancellationToken.None);

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Cancelled);
        var seat = await Db.ShowtimeSeats.SingleAsync(ss => ss.ShowtimeId == booking.ShowtimeId);
        seat.Status.ShouldBe(ShowtimeSeatStatus.Available);
    }

    [Fact]
    public async Task ProcessPaymentFailureAsync_BookingAlreadyConfirmed_DoesNothing()
    {
        TestDataSeeder.SetCinemaSettings(Db);
        var (booking, _, _) = await SeedHeldBookingAsync();
        var confirmSut = CreateSut();
        await confirmSut.ConfirmBookingAndGenerateTicketsAsync(
            booking.BookingId,
            CancellationToken.None
        );
        var sut = CreateSut();

        await sut.ProcessPaymentFailureAsync(booking.BookingId, CancellationToken.None);

        var reloaded = await Db.Bookings.SingleAsync(b => b.BookingId == booking.BookingId);
        reloaded.Status.ShouldBe(BookingStatus.Confirmed);
    }
}
