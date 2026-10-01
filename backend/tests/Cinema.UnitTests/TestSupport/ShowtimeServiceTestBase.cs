using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
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
        Db.ChangeTracker.Clear();

        return new(
            Db,
            Options.Create(new BookingOptions { RoomBufferMinutes = RoomBufferMinutes }),
            PaymentGatewayMocks.Select(m => m.Object),
            BackgroundJobClientMock.Object,
            BookingServiceMock.Object,
            NullLogger<ShowtimeService>.Instance
        );
    }

    public void Dispose() => _handle.Dispose();
}
