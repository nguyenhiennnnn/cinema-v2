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

public class BookingCheckoutServiceTests : BookingServiceTestBase
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

        var holdingSut = CreateHoldSut();
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
        var sut = CreateCheckoutSut();

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
        var sut = CreateCheckoutSut();

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
        var sut = CreateCheckoutSut();

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
        var sut = CreateCheckoutSut();

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
        var sut = CreateCheckoutSut();

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
