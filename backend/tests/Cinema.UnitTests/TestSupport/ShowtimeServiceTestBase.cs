using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

public abstract class ShowtimeServiceTestBase : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    protected AppDbContext Db => _handle.Db;
    protected int RoomBufferMinutes { get; set; } = 15;
    protected Mock<IBackgroundJobClient> BackgroundJobClientMock { get; } = new();
    protected Mock<IBookingService> BookingServiceMock { get; } = new();
    protected List<Mock<IPaymentGateway>> PaymentGatewayMocks { get; } = [];

    protected Mock<IPaymentGateway> AddPaymentGateway(string methodName)
    {
        var mock = new Mock<IPaymentGateway>();
        mock.SetupGet(g => g.MethodName).Returns(methodName);
        PaymentGatewayMocks.Add(mock);
        return mock;
    }

    protected ShowtimeService CreateSut()
    {
        // Khoảng nghỉ phòng nay là chính sách trong CinemaSettings (DB), không còn nằm ở BookingOptions.
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

        return new(
            Db,
            PaymentGatewayMocks.Select(m => m.Object),
            BackgroundJobClientMock.Object,
            new NotificationService(BackgroundJobClientMock.Object, Options.Create(new BrandOptions())),
            BookingServiceMock.Object,
            NullLogger<ShowtimeService>.Instance
        );
    }

    public void Dispose() => _handle.Dispose();
}
