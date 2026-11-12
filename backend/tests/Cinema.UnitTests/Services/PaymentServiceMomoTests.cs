using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.Notifications;
using Cinema.Application.Services.Payments;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Constants;
using Cinema.UnitTests.TestSupport;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class PaymentServiceMomoTests : IDisposable
{
    private readonly PostgresAppDbContextHandle _handle = PostgresAppDbContextHandle.Create();
    private readonly Mock<IBookingFulfillmentService> _bookingServiceMock = new();
    private readonly Mock<IPaymentGateway> _gatewayMock = new();
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock = new();

    private PaymentService CreateSut(bool withGateway = true)
    {
        _handle.Db.ChangeTracker.Clear();
        IEnumerable<IPaymentGateway> gateways = [];
        if (withGateway)
        {
            _gatewayMock.SetupGet(g => g.MethodName).Returns(PaymentMethodNames.Momo);
            gateways = [_gatewayMock.Object];
        }

        return new PaymentService(
            _handle.Db,
            _bookingServiceMock.Object,
            gateways,
            new NotificationService(
                _backgroundJobClientMock.Object,
                Options.Create(new BrandOptions())
            ),
            NullLogger<PaymentService>.Instance
        );
    }

    private async Task<Payment> SeedPendingPaymentAsync(
        decimal amount = 100_000m,
        string transactionId = "MOMO-1"
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
            BookingCode = "BK20260101MOMO",
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
            PaymentMethod = PaymentMethod.Momo,
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
        string responseCode = "0"
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

    private void VerifyConfirmCalls(Times times) =>
        _bookingServiceMock.Verify(
            b =>
                b.ConfirmBookingAndGenerateTicketsAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()
                ),
            times
        );

    [Fact]
    public async Task ProcessMomoIpnAsync_ValidSuccessfulCallback_MarksPaymentSuccessAndConfirmsBooking()
    {
        var payment = await SeedPendingPaymentAsync();
        SetupCallback(true, true, 100_000m, "MOMO-1");
        var sut = CreateSut();

        var result = await sut.ProcessMomoIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.ResultCode.ShouldBe(0);
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
    public async Task ProcessMomoIpnAsync_InvalidSignature_Returns97AndDoesNotTouchPayment()
    {
        var payment = await SeedPendingPaymentAsync();
        SetupCallback(false, true, 100_000m, "MOMO-1");
        var sut = CreateSut();

        var result = await sut.ProcessMomoIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.ResultCode.ShouldBe(97);
        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Pending);
        VerifyConfirmCalls(Times.Never());
    }

    [Fact]
    public async Task ProcessMomoIpnAsync_UnknownTransactionId_Returns1()
    {
        SetupCallback(true, true, 100_000m, "DOES-NOT-EXIST");
        var sut = CreateSut();

        var result = await sut.ProcessMomoIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.ResultCode.ShouldBe(1);
    }

    [Fact]
    public async Task ProcessMomoIpnAsync_AmountMismatch_Returns4AndLeavesPaymentPending()
    {
        var payment = await SeedPendingPaymentAsync(amount: 100_000m);
        SetupCallback(true, true, 50_000m, "MOMO-1");
        var sut = CreateSut();

        var result = await sut.ProcessMomoIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.ResultCode.ShouldBe(4);
        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Pending);
        VerifyConfirmCalls(Times.Never());
    }

    [Fact]
    public async Task ProcessMomoIpnAsync_AlreadySuccessfulPayment_IsIdempotentAndDoesNotReconfirmBooking()
    {
        var payment = await SeedPendingPaymentAsync();
        payment.Status = PaymentStatus.Success;
        await _handle.Db.SaveChangesAsync();
        SetupCallback(true, true, 100_000m, "MOMO-1");
        var sut = CreateSut();

        var result = await sut.ProcessMomoIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.ResultCode.ShouldBe(0);
        VerifyConfirmCalls(Times.Never());
    }

    [Fact]
    public async Task ProcessMomoIpnAsync_DuplicateDeliveryOfSuccessfulCallback_ConfirmsBookingOnlyOnce()
    {
        await SeedPendingPaymentAsync();
        SetupCallback(true, true, 100_000m, "MOMO-1");

        var first = await CreateSut()
            .ProcessMomoIpnAsync(new Dictionary<string, string>(), CancellationToken.None);
        var second = await CreateSut()
            .ProcessMomoIpnAsync(new Dictionary<string, string>(), CancellationToken.None);

        first.ResultCode.ShouldBe(0);
        second.ResultCode.ShouldBe(0);
        VerifyConfirmCalls(Times.Once());
    }

    [Fact]
    public async Task ProcessMomoIpnAsync_FailedPaymentCallback_MarksPaymentFailedAndProcessesFailure()
    {
        var payment = await SeedPendingPaymentAsync();
        SetupCallback(true, false, 100_000m, "MOMO-1", responseCode: "1006");
        var sut = CreateSut();

        await sut.ProcessMomoIpnAsync(new Dictionary<string, string>(), CancellationToken.None);

        var reloaded = await _handle.Db.Payments.SingleAsync(p => p.PaymentId == payment.PaymentId);
        reloaded.Status.ShouldBe(PaymentStatus.Failed);
        _bookingServiceMock.Verify(
            b => b.ProcessPaymentFailureAsync(payment.BookingId, It.IsAny<CancellationToken>()),
            Times.Once
        );
        VerifyConfirmCalls(Times.Never());
    }

    [Fact]
    public async Task ProcessMomoIpnAsync_GatewayNotConfigured_Returns99WithoutThrowing()
    {
        var sut = CreateSut(withGateway: false);

        var result = await sut.ProcessMomoIpnAsync(
            new Dictionary<string, string>(),
            CancellationToken.None
        );

        result.ResultCode.ShouldBe(99);
    }

    public void Dispose() => _handle.Dispose();
}
