using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.Notifications;
using Cinema.Application.Services.Showtimes;
using Cinema.Infrastructure.Persistence;
using Hangfire;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Cinema.UnitTests.TestSupport;

public abstract class ShowtimeServiceTestBase(PostgresContainerFixture fixture) : IDisposable
{
    private readonly PostgresAppDbContextHandle _handle = new(fixture.MasterConnectionString);

    protected AppDbContext Db => _handle.Db;
    protected int RoomBufferMinutes { get; set; } = 15;
    protected Mock<IBackgroundJobClient> BackgroundJobClientMock { get; } = new();
    protected Mock<IBookingFulfillmentService> BookingServiceMock { get; } = new();
    protected List<Mock<IPaymentGateway>> PaymentGatewayMocks { get; } = [];

    protected Mock<IPaymentGateway> AddPaymentGateway(string methodName)
    {
        var mock = new Mock<IPaymentGateway>();
        mock.SetupGet(g => g.MethodName).Returns(methodName);
        PaymentGatewayMocks.Add(mock);
        return mock;
    }

    protected ShowtimeService CreateShowtimeService()
    {
        PrepareDatabase();
        return new(Db, NullLogger<ShowtimeService>.Instance);
    }

    protected ShowtimeCancellationService CreateCancellationService()
    {
        PrepareDatabase();
        return NewCancellationService();
    }

    protected ShowtimeBatchService CreateBatchService()
    {
        PrepareDatabase();
        return NewBatchService();
    }

    protected ShowtimeAutoScheduleService CreateAutoScheduleService()
    {
        PrepareDatabase();
        return new(Db, NewBatchService(), NullLogger<ShowtimeAutoScheduleService>.Instance);
    }

    protected ShowtimeLifecycleService CreateLifecycleService()
    {
        PrepareDatabase();
        return new(Db, NullLogger<ShowtimeLifecycleService>.Instance);
    }

    private ShowtimeCancellationService NewCancellationService() =>
        new(
            Db,
            BookingServiceMock.Object,
            new NotificationService(
                BackgroundJobClientMock.Object,
                Options.Create(new BrandOptions())
            ),
            BackgroundJobClientMock.Object,
            NullLogger<ShowtimeCancellationService>.Instance
        );

    private ShowtimeBatchService NewBatchService() =>
        new(Db, NewCancellationService(), NullLogger<ShowtimeBatchService>.Instance);

    private void PrepareDatabase()
    {
        var settings = Db.CinemaSettings.FirstOrDefault(s => s.Id == 1);
        if (settings is null)
        {
            settings = new Cinema.Domain.Entities.CinemaSettings
            {
                Id = 1,
                Name = "Test Cinema",
                Address = "N/A",
            };
            Db.CinemaSettings.Add(settings);
        }
        settings.RoomBufferMinutes = RoomBufferMinutes;
        Db.SaveChanges();
        Db.ChangeTracker.Clear();
    }

    public void Dispose() => _handle.Dispose();
}
