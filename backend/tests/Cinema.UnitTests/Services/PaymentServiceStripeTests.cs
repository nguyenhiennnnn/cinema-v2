using Cinema.Application.Interfaces;
using Cinema.Application.Services.Payments;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class PaymentServiceStripeTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();
    private readonly Mock<IBookingService> _bookingServiceMock = new();
    private readonly Mock<IPaymentGateway> _gatewayMock = new();
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock = new();

    private PaymentService CreateSut(bool withGateway = true)
    {
        _handle.Db.ChangeTracker.Clear();
        IEnumerable<IPaymentGateway> gateways = [];
        if (withGateway)
        {
            _gatewayMock.SetupGet(g => g.MethodName).Returns(PaymentMethodNames.Stripe);
            gateways = [_gatewayMock.Object];
        }

        return new PaymentService(
            _handle.Db,
            _bookingServiceMock.Object,
            gateways,
            _backgroundJobClientMock.Object,
            NullLogger<PaymentService>.Instance
        );
    }

    private async Task<Payment> SeedPendingPaymentAsync(
        decimal amount = 100_000m,
        string transactionId = "PI-1"
    )
    {
        var customer = TestDataSeeder.AddCustomer(_handle.Db);
        var room = TestDataSeeder.AddRoom(_handle.Db);
        var seats = TestDataSeeder.AddSeats(_handle.Db, room.RoomId, count: 1);
        var movie = TestDataSeeder.AddMovie(_handle.Db);
        var showtime = TestDataSeeder.AddShowtime(_handle.Db, movie, room, seats);
        var booking = new Booking
        {
            UserId = customer.UserId,
            ShowtimeId = showtime.ShowtimeId,
            BookingCode = "BK20260101STRP",
            CustomerName = "Nguyen Van A",
            TotalAmount = amount,
            FinalAmount = amount,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        };
        _handle.Db.Bookings.Add(booking);
        await _handle.Db.SaveChangesAsync();

        var payment = new Payment
        {
            BookingId = booking.BookingId,
            PaymentMethod = PaymentMethod.Stripe,
            Status = PaymentStatus.Pending,
            Amount = amount,
            TransactionId = transactionId,
            CreatedAt = DateTime.UtcNow,
        };
        _handle.Db.Payments.Add(payment);
        await _handle.Db.SaveChangesAsync();
        return payment;
    }

    private void SetupWebhook(
        bool signatureValid,
        bool isSuccess,
        decimal amount,
        string transactionId,
        string responseCode = "succeeded"
    ) =>
        _gatewayMock
            .Setup(g => g.VerifyWebhook(It.IsAny<string>(), It.IsAny<string?>()))
            .Returns(
                new GatewayCallbackResult(
                    signatureValid,
                    isSuccess,
                    transactionId,
                    responseCode,
                    amount,
                    0
                )
            );

    [Fact]
    public async Task ProcessStripeWebhookAsync_ValidSuccessfulEvent_MarksPaymentSuccessAndConfirmsBooking()
    {
        var payment = await SeedPendingPaymentAsync(amount: 100_000m, transactionId: "PI-1");
        SetupWebhook(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "PI-1"
        );
        var sut = CreateSut();

        var result = await sut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        result.Success.ShouldBeTrue();
        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Success);
        reloaded.PaidAt.ShouldNotBeNull();
        _bookingServiceMock.Verify(
            b =>
                b.ConfirmBookingAndGenerateTicketsAsync(
                    payment.BookingId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_InvalidSignature_ReturnsFailureAndDoesNotTouchPayment()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "PI-2");
        SetupWebhook(
            signatureValid: false,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "PI-2"
        );
        var sut = CreateSut();

        var result = await sut.ProcessStripeWebhookAsync("{}", "bad-sig", CancellationToken.None);

        result.Success.ShouldBeFalse();
        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Pending);
        _bookingServiceMock.Verify(
            b =>
                b.ConfirmBookingAndGenerateTicketsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_UnrelatedEventWithNoTransactionId_IsIgnoredSuccessfully()
    {
        SetupWebhook(signatureValid: true, isSuccess: false, amount: 0m, transactionId: "");
        var sut = CreateSut();

        var result = await sut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        result.Success.ShouldBeTrue();
        result.Message.ShouldBe("Event ignored");
        _bookingServiceMock.Verify(
            b =>
                b.ConfirmBookingAndGenerateTicketsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
        _bookingServiceMock.Verify(
            b => b.ProcessPaymentFailureAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_UnknownTransactionId_ReturnsPaymentNotFound()
    {
        SetupWebhook(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "DOES-NOT-EXIST"
        );
        var sut = CreateSut();

        var result = await sut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Payment not found");
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_GatewayNotConfigured_ReturnsFailureWithoutThrowing()
    {
        var sut = CreateSut(withGateway: false);

        var result = await sut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        result.Success.ShouldBeFalse();
        result.Message.ShouldBe("Gateway not configured");
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_FailedPaymentEvent_MarksPaymentFailedAndReleasesBooking()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "PI-5");
        SetupWebhook(
            signatureValid: true,
            isSuccess: false,
            amount: 100_000m,
            transactionId: "PI-5",
            responseCode: "payment_failed"
        );
        var sut = CreateSut();

        await sut.ProcessStripeWebhookAsync("{}", "sig-header", CancellationToken.None);

        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Failed);
        _bookingServiceMock.Verify(
            b => b.ProcessPaymentFailureAsync(payment.BookingId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    // ── Idempotency ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ProcessStripeWebhookAsync_PaymentAlreadySuccessful_IsIdempotentAndDoesNotReconfirmBooking()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "PI-IDEMPOTENT-1");
        payment.Status = PaymentStatus.Success;
        await _handle.Db.SaveChangesAsync();
        SetupWebhook(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "PI-IDEMPOTENT-1"
        );
        var sut = CreateSut();

        var result = await sut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        result.Success.ShouldBeTrue();
        result.Message.ShouldBe("Payment already processed");
        _bookingServiceMock.Verify(
            b =>
                b.ConfirmBookingAndGenerateTicketsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_PaymentAlreadyFailed_IsIdempotentAndDoesNotReprocessFailure()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "PI-IDEMPOTENT-2");
        payment.Status = PaymentStatus.Failed;
        await _handle.Db.SaveChangesAsync();
        SetupWebhook(
            signatureValid: true,
            isSuccess: false,
            amount: 100_000m,
            transactionId: "PI-IDEMPOTENT-2"
        );
        var sut = CreateSut();

        var result = await sut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        result.Success.ShouldBeTrue();
        result.Message.ShouldBe("Payment already processed");
        _bookingServiceMock.Verify(
            b => b.ProcessPaymentFailureAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_DuplicateWebhookDeliveryForSameSuccessfulEvent_ConfirmsBookingOnlyOnce()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "PI-DUPLICATE-1");
        SetupWebhook(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "PI-DUPLICATE-1"
        );

        var firstSut = CreateSut();
        var firstResult = await firstSut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        var secondSut = CreateSut();
        var secondResult = await secondSut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        firstResult.Success.ShouldBeTrue();
        secondResult.Success.ShouldBeTrue();
        secondResult.Message.ShouldBe("Payment already processed");
        _bookingServiceMock.Verify(
            b =>
                b.ConfirmBookingAndGenerateTicketsAsync(
                    payment.BookingId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessStripeWebhookAsync_SeatsExpiredBeforeConfirmation_SwallowsExceptionAndStillReturnsSuccessToGateway()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "PI-6");
        SetupWebhook(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "PI-6"
        );
        _bookingServiceMock
            .Setup(b =>
                b.ConfirmBookingAndGenerateTicketsAsync(
                    payment.BookingId,
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new SeatsNoLongerHeldException(payment.BookingId));
        var sut = CreateSut();

        var result = await sut.ProcessStripeWebhookAsync(
            "{}",
            "sig-header",
            CancellationToken.None
        );

        result.Success.ShouldBeTrue();
        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Success);
    }

    public void Dispose() => _handle.Dispose();
}
