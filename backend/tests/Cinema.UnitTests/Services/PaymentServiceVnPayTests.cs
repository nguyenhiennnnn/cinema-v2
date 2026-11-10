using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.Notifications;
using Cinema.Application.Services.Payments;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class PaymentServiceVnPayTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();
    private readonly Mock<IBookingService> _bookingServiceMock = new();
    private readonly Mock<IPaymentGateway> _gatewayMock = new();
    private readonly Mock<IBackgroundJobClient> backgroundJobClientMock = new();

    private PaymentService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        _gatewayMock.SetupGet(g => g.MethodName).Returns(PaymentMethodNames.VnPay);
        return new PaymentService(
            _handle.Db,
            _bookingServiceMock.Object,
            [_gatewayMock.Object],
            new NotificationService(backgroundJobClientMock.Object, Options.Create(new BrandOptions())),
            NullLogger<PaymentService>.Instance
        );
    }

    private async Task<Payment> SeedPendingPaymentAsync(
        decimal amount = 100_000m,
        string transactionId = "TXN-1"
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
            BookingCode = "BK20260101ABCD",
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
            PaymentMethod = PaymentMethod.VNPay,
            Status = PaymentStatus.Pending,
            Amount = amount,
            TransactionId = transactionId,
            CreatedAt = DateTime.UtcNow,
        };
        _handle.Db.Payments.Add(payment);
        await _handle.Db.SaveChangesAsync();
        return payment;
    }

    private void SetupCallback(
        bool signatureValid,
        bool isSuccess,
        decimal amount,
        string transactionId,
        string responseCode = "00"
    ) =>
        _gatewayMock
            .Setup(g => g.VerifyCallback(It.IsAny<IReadOnlyDictionary<string, string>>()))
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
    public async Task ProcessVnPayIpnAsync_ValidSuccessfulCallback_MarksPaymentSuccessAndConfirmsBooking()
    {
        var payment = await SeedPendingPaymentAsync(amount: 100_000m, transactionId: "TXN-1");
        SetupCallback(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "TXN-1"
        );
        var sut = CreateSut();

        var result = await sut.ProcessVnPayIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.RspCode.ShouldBe("00");
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
    public async Task ProcessVnPayIpnAsync_InvalidSignature_ReturnsErrorAndDoesNotTouchPayment()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "TXN-2");
        SetupCallback(
            signatureValid: false,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "TXN-2"
        );
        var sut = CreateSut();

        var result = await sut.ProcessVnPayIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.RspCode.ShouldBe("97");
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
    public async Task ProcessVnPayIpnAsync_UnknownTransactionId_ReturnsOrderNotFound()
    {
        SetupCallback(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "DOES-NOT-EXIST"
        );
        var sut = CreateSut();

        var result = await sut.ProcessVnPayIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.RspCode.ShouldBe("01");
    }

    [Fact]
    public async Task ProcessVnPayIpnAsync_AmountMismatch_ReturnsInvalidAmountAndLeavesPaymentPending()
    {
        var payment = await SeedPendingPaymentAsync(amount: 100_000m, transactionId: "TXN-3");
        SetupCallback(
            signatureValid: true,
            isSuccess: true,
            amount: 50_000m,
            transactionId: "TXN-3"
        );
        var sut = CreateSut();

        var result = await sut.ProcessVnPayIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.RspCode.ShouldBe("04");
        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Pending);
    }

    [Fact]
    public async Task ProcessVnPayIpnAsync_AlreadySuccessfulPayment_IsIdempotentAndDoesNotReconfirmBooking()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "TXN-4");
        payment.Status = PaymentStatus.Success;
        await _handle.Db.SaveChangesAsync();
        SetupCallback(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "TXN-4"
        );
        var sut = CreateSut();

        var result = await sut.ProcessVnPayIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.RspCode.ShouldBe("00");
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
    public async Task ProcessVnPayIpnAsync_FailedPaymentCallback_MarksPaymentFailedAndReleasesBooking()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "TXN-5");
        SetupCallback(
            signatureValid: true,
            isSuccess: false,
            amount: 100_000m,
            transactionId: "TXN-5",
            responseCode: "24"
        );
        var sut = CreateSut();

        await sut.ProcessVnPayIpnAsync(new Dictionary<string, string>(), CancellationToken.None);

        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Failed);
        _bookingServiceMock.Verify(
            b => b.ProcessPaymentFailureAsync(payment.BookingId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task ProcessVnPayIpnAsync_GatewayNotConfigured_ReturnsGatewayNotConfiguredWithoutThrowing()
    {
        _handle.Db.ChangeTracker.Clear();
        var sutWithoutGateway = new PaymentService(
            _handle.Db,
            _bookingServiceMock.Object,
            [], 
            new NotificationService(backgroundJobClientMock.Object, Options.Create(new BrandOptions())),
            NullLogger<PaymentService>.Instance
        );

        var result = await sutWithoutGateway.ProcessVnPayIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.RspCode.ShouldBe("99");
    }

    [Fact]
    public async Task ProcessVnPayIpnAsync_SeatsExpiredBeforeConfirmation_SwallowsExceptionAndStillReturnsSuccessToGateway()
    {
        var payment = await SeedPendingPaymentAsync(transactionId: "TXN-6");
        SetupCallback(
            signatureValid: true,
            isSuccess: true,
            amount: 100_000m,
            transactionId: "TXN-6"
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

        var result = await sut.ProcessVnPayIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.RspCode.ShouldBe("00");
        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Success);
    }

    public void Dispose() => _handle.Dispose();
}
