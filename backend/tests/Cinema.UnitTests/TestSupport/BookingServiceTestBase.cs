using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.Bookings;
using Cinema.Application.Services.Notifications;
using Cinema.Infrastructure.Persistence;
using Hangfire;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Cinema.UnitTests.TestSupport;

public abstract class BookingServiceTestBase : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    protected AppDbContext Db => _handle.Db;
    protected Mock<IRealtimeNotifier> RealtimeNotifierMock { get; } = new();
    protected Mock<IBackgroundJobClient> BackgroundJobClientMock { get; } = new();
    protected List<Mock<IPaymentGateway>> PaymentGatewayMocks { get; } = [];
    protected int HoldDurationMinutes { get; set; } = 15;

    protected BookingServiceTestBase()
    {
        RealtimeNotifierMock
            .Setup(n =>
                n.NotifySeatHeldAsync(
                    It.IsAny<int>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        RealtimeNotifierMock
            .Setup(n =>
                n.NotifySeatReleasedAsync(
                    It.IsAny<int>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        RealtimeNotifierMock
            .Setup(n =>
                n.NotifySeatBookedAsync(
                    It.IsAny<int>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        RealtimeNotifierMock
            .Setup(n =>
                n.NotifyHoldExpiredAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<List<int>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
    }

    protected Mock<IPaymentGateway> AddPaymentGateway(string methodName)
    {
        var mock = new Mock<IPaymentGateway>();
        mock.SetupGet(g => g.MethodName).Returns(methodName);
        PaymentGatewayMocks.Add(mock);
        return mock;
    }

    private void PrepareContext()
    {
        var settings = Db.CinemaSettings.FirstOrDefault(s => s.Id == 1);
        if (settings is not null && settings.HoldDurationMinutes != HoldDurationMinutes)
        {
            settings.HoldDurationMinutes = HoldDurationMinutes;
            Db.SaveChanges();
        }

        Db.ChangeTracker.Clear();
    }

    private BookingSeatReleaser CreateReleaser() => new(Db, RealtimeNotifierMock.Object);

    private BookingFulfillmentService CreateFulfillment() =>
        new(
            Db,
            Options.Create(new BookingOptions()),
            RealtimeNotifierMock.Object,
            new NotificationService(BackgroundJobClientMock.Object, Options.Create(new BrandOptions())),
            CreateReleaser(),
            NullLogger<BookingFulfillmentService>.Instance
        );

    protected BookingQueryService CreateQuerySut()
    {
        PrepareContext();
        return new(Db, Options.Create(new BookingOptions()));
    }

    protected BookingHoldService CreateHoldSut()
    {
        PrepareContext();
        return new(
            Db,
            RealtimeNotifierMock.Object,
            [new OnlineHoldPolicy(Db), new CounterHoldPolicy(Db)],
            CreateFulfillment(),
            NullLogger<BookingHoldService>.Instance
        );
    }

    protected BookingCheckoutService CreateCheckoutSut()
    {
        PrepareContext();
        return new(
            Db,
            PaymentGatewayMocks.Select(m => m.Object),
            NullLogger<BookingCheckoutService>.Instance
        );
    }

    protected CounterBookingService CreateCounterSut()
    {
        PrepareContext();
        return new(
            Db,
            CreateHoldSut(),
            CreateFulfillment(),
            CreateReleaser(),
            NullLogger<CounterBookingService>.Instance
        );
    }

    protected BookingCancellationService CreateCancellationSut()
    {
        PrepareContext();
        return new(
            Db,
            RealtimeNotifierMock.Object,
            PaymentGatewayMocks.Select(m => m.Object),
            new NotificationService(BackgroundJobClientMock.Object, Options.Create(new BrandOptions())),
            CreateFulfillment(),
            CreateReleaser(),
            NullLogger<BookingCancellationService>.Instance
        );
    }

    protected BookingFulfillmentService CreateFulfillmentSut()
    {
        PrepareContext();
        return CreateFulfillment();
    }

    public void Dispose() => _handle.Dispose();
}
