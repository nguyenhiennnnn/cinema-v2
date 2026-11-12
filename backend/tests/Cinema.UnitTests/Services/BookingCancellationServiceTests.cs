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

public class BookingCancellationServiceTests(PostgresContainerFixture fixture) : BookingServiceTestBase(fixture)
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
    public async Task CancelBookingAsync_PendingBooking_ReleasesSeatsAndCancelsWithoutRefund()
    {
        var (booking, customer, _) = await SeedHeldBookingAsync();
        var sut = CreateCancellationSut();

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
        var confirmSut = CreateFulfillmentSut();
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
        var sut = CreateCancellationSut();

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
        var confirmSut = CreateFulfillmentSut();
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
        var sut = CreateCancellationSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CancelBookingAsync(customer.UserId, booking.BookingId, CancellationToken.None)
        );
    }

    [Fact]
    public async Task CancelBookingAsync_ConfirmedBookingZeroRefundPercent_ThrowsBusinessRuleException()
    {
        TestDataSeeder.SetCinemaSettings(Db, cancelRefundPercent: 0m, cancelDeadlineHours: 2);
        var (booking, customer, _) = await SeedHeldBookingAsync(
            startTime: DateTime.UtcNow.AddDays(2)
        );
        var confirmSut = CreateFulfillmentSut();
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
        var sut = CreateCancellationSut();

        var ex = await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.CancelBookingAsync(customer.UserId, booking.BookingId, CancellationToken.None)
        );
        ex.Message.ShouldBe("Rạp hiện không hỗ trợ hoàn tiền.");
    }
}
