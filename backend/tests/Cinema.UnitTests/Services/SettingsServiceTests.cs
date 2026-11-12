using Cinema.Application.DTOs.Settings;
using Cinema.Application.Services.Settings;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class SettingsServiceTests(PostgresContainerFixture fixture) : IDisposable
{
    private readonly PostgresAppDbContextHandle _handle = new(fixture.MasterConnectionString);

    private SettingsService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        BusinessTime.Configure("Asia/Ho_Chi_Minh");
        return new SettingsService(
            _handle.Db,
            Microsoft.Extensions.Options.Options.Create(
                new Cinema.Application.Options.BrandOptions
                {
                    TimeZoneId = "Asia/Ho_Chi_Minh",
                    Locale = "vi-VN",
                    CurrencyCode = "VND",
                    CurrencySymbol = "VNÄ",
                }
            )
        );
    }

    private static UpdateSettingsRequest Request(
        decimal cancelRefundPercent = 80m,
        int cancelDeadlineHours = 3,
        string? openingTime = "09:00",
        string? closingTime = "23:30",
        int? holdDurationMinutes = null,
        int? checkInOpenMinutes = null,
        string? seatGapRuleMode = null
    ) =>
        new(
            "Perfo Cinema",
            "1 Nguyen Hue",
            "0900000000",
            "hello@perfo.local",
            "0312345678",
            null,
            openingTime,
            closingTime,
            20_000m,
            cancelRefundPercent,
            cancelDeadlineHours,
            30,
            holdDurationMinutes,
            checkInOpenMinutes,
            seatGapRuleMode
        );

    [Fact]
    public async Task GetSettingsAsync_SettingsRowExists_ReturnsSettings()
    {
        var sut = CreateSut();

        var result = await sut.GetSettingsAsync(CancellationToken.None);

        result.Name.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetSettingsAsync_ReturnsBusinessTimeZoneAsIanaId()
    {
        var sut = CreateSut();

        var result = await sut.GetSettingsAsync(CancellationToken.None);

        result.TimeZoneId.ShouldBe(BusinessTime.IanaId);
        result.TimeZoneId.ShouldContain("/");
    }

    [Fact]
    public async Task GetSettingsAsync_SettingsRowMissing_ThrowsNotFound()
    {
        _handle.Db.CinemaSettings.RemoveRange(_handle.Db.CinemaSettings);
        await _handle.Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<NotFoundException>(() =>
            sut.GetSettingsAsync(CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateSettingsAsync_ValidRequest_PersistsAllFields()
    {
        var sut = CreateSut();

        await sut.UpdateSettingsAsync(
            Request(
                holdDurationMinutes: 10,
                checkInOpenMinutes: 45,
                seatGapRuleMode: nameof(SeatGapRuleMode.AllowEdge)
            ),
            CancellationToken.None
        );

        var saved = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        saved.Name.ShouldBe("Perfo Cinema");
        saved.OpeningTime.ShouldBe(new TimeOnly(9, 0));
        saved.ClosingTime.ShouldBe(new TimeOnly(23, 30));
        saved.PointsPerAmount.ShouldBe(20_000m);
        saved.CancelRefundPercent.ShouldBe(80m);
        saved.CancelDeadlineHours.ShouldBe(3);
        saved.StopSellingMinutesBefore.ShouldBe(30);
        saved.HoldDurationMinutes.ShouldBe(10);
        saved.CheckInOpenMinutes.ShouldBe(45);
        saved.SeatGapRuleMode.ShouldBe(SeatGapRuleMode.AllowEdge);
        saved.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task UpdateSettingsAsync_OptionalFieldsOmitted_KeepsExistingValues()
    {
        TestDataSeeder.SetCinemaSettings(
            _handle.Db,
            holdDurationMinutes: 12,
            checkInOpenMinutes: 50
        );
        var before = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        var sut = CreateSut();

        await sut.UpdateSettingsAsync(Request(), CancellationToken.None);

        var saved = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        saved.HoldDurationMinutes.ShouldBe(12);
        saved.CheckInOpenMinutes.ShouldBe(50);
        saved.SeatGapRuleMode.ShouldBe(before.SeatGapRuleMode);
    }

    [Fact]
    public async Task UpdateSettingsAsync_UnknownSeatGapRuleMode_KeepsExistingMode()
    {
        var before = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        var sut = CreateSut();

        await sut.UpdateSettingsAsync(Request(seatGapRuleMode: "NotAMode"), CancellationToken.None);

        var saved = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        saved.SeatGapRuleMode.ShouldBe(before.SeatGapRuleMode);
    }

    [Fact]
    public async Task UpdateSettingsAsync_BlankOperatingHours_ClearsThem()
    {
        var sut = CreateSut();

        await sut.UpdateSettingsAsync(
            Request(openingTime: null, closingTime: " "),
            CancellationToken.None
        );

        var saved = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        saved.OpeningTime.ShouldBeNull();
        saved.ClosingTime.ShouldBeNull();
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(100.01)]
    public async Task UpdateSettingsAsync_RefundPercentOutOfRange_ThrowsAndDoesNotPersist(
        double percent
    )
    {
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.UpdateSettingsAsync(
                Request(cancelRefundPercent: (decimal)percent),
                CancellationToken.None
            )
        );

        var saved = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        saved.Name.ShouldNotBe("Perfo Cinema");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public async Task UpdateSettingsAsync_RefundPercentAtBoundary_Succeeds(int percent)
    {
        var sut = CreateSut();

        await sut.UpdateSettingsAsync(
            Request(cancelRefundPercent: percent),
            CancellationToken.None
        );

        var saved = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        saved.CancelRefundPercent.ShouldBe(percent);
    }

    [Fact]
    public async Task UpdateSettingsAsync_NegativeCancelDeadlineHours_Throws()
    {
        var sut = CreateSut();

        await Should.ThrowAsync<BusinessRuleException>(() =>
            sut.UpdateSettingsAsync(Request(cancelDeadlineHours: -1), CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateSettingsAsync_InvalidOperatingTimeFormat_Throws()
    {
        var sut = CreateSut();

        await Should.ThrowAsync<FormatException>(() =>
            sut.UpdateSettingsAsync(Request(openingTime: "9am"), CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateSettingsAsync_SettingsRowMissing_ThrowsNotFound()
    {
        _handle.Db.CinemaSettings.RemoveRange(_handle.Db.CinemaSettings);
        await _handle.Db.SaveChangesAsync();
        var sut = CreateSut();

        await Should.ThrowAsync<NotFoundException>(() =>
            sut.UpdateSettingsAsync(Request(), CancellationToken.None)
        );
    }

    public void Dispose() => _handle.Dispose();
}
