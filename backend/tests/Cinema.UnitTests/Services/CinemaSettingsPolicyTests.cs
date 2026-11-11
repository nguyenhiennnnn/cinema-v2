using Cinema.Application.DTOs.Bookings;
using Cinema.Application.DTOs.Settings;
using Cinema.Application.Interfaces;
using Cinema.Application.Services.Settings;
using Cinema.Application.Utils;
using Cinema.Application.Validators.Settings;
using Cinema.Domain.Entities;
using Cinema.Shared.Exceptions;
using Cinema.UnitTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Cinema.UnitTests.Services;

public class BookingLimitsFromSettingsTests : BookingServiceTestBase
{
    private async Task SetLimitsAsync(int maxSeats, int maxCombo)
    {
        var settings = await Db.CinemaSettings.SingleAsync(s => s.Id == 1);
        settings.MaxSeatsPerBooking = maxSeats;
        settings.MaxComboQuantity = maxCombo;
        await Db.SaveChangesAsync();
    }

    [Fact]
    public async Task HoldSeatsAsync_SeatCountAboveSettingsLimit_ThrowsValidation()
    {
        await SetLimitsAsync(maxSeats: 1, maxCombo: 10);

        var ex = await Should.ThrowAsync<ValidationAppException>(() =>
            CreateHoldSut()
                .HoldSeatsAsync(
                    userId: 1,
                    new HoldSeatsRequest(1, "Nguyen Van A", null, [1, 2], null),
                    CancellationToken.None
                )
        );

        ex.Errors!.ShouldContainKey("seatIds");
    }

    [Fact]
    public async Task HoldSeatsAsync_ComboQuantityAboveSettingsLimit_ThrowsValidation()
    {
        await SetLimitsAsync(maxSeats: 10, maxCombo: 2);

        var ex = await Should.ThrowAsync<ValidationAppException>(() =>
            CreateHoldSut()
                .HoldSeatsAsync(
                    userId: 1,
                    new HoldSeatsRequest(
                        1,
                        "Nguyen Van A",
                        null,
                        [1],
                        [new ComboSelectionRequest(1, 3)]
                    ),
                    CancellationToken.None
                )
        );

        ex.Errors!.ShouldContainKey("combos");
    }
}

public class LoyaltyWindowSettingsTests : BookingServiceTestBase
{
    [Fact]
    public async Task SumActiveAsync_UsesLoyaltyWindowMonthsFromSettings()
    {
        var customer = TestDataSeeder.AddCustomer(Db);
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 100, DateTime.UtcNow.AddMonths(-2));
        LoyaltyTestHelper.AddEntry(Db, customer.UserId, null, 40, DateTime.UtcNow.AddMonths(-5));
        await Db.SaveChangesAsync();

        // Mặc định 12 tháng: cả hai dòng còn hiệu lực.
        (await LoyaltyWindow.SumActiveAsync(Db, customer.UserId, DateTime.UtcNow, default)).ShouldBe(140);

        var settings = await Db.CinemaSettings.SingleAsync(s => s.Id == 1);
        settings.LoyaltyWindowMonths = 3;
        await Db.SaveChangesAsync();

        // Cửa sổ 3 tháng: dòng 5 tháng trước đã hết hạn.
        (await LoyaltyWindow.SumActiveAsync(Db, customer.UserId, DateTime.UtcNow, default)).ShouldBe(100);
    }

    [Fact]
    public void Cutoff_UsesGivenMonths()
    {
        var now = new DateTime(2026, 11, 6, 0, 0, 0, DateTimeKind.Utc);

        LoyaltyWindow.Cutoff(now, 6).ShouldBe(now.AddMonths(-6));
        LoyaltyWindow.Cutoff(now).ShouldBe(now.AddMonths(-12));
    }
}

public class ShowtimePolicySettingsTests : BookingServiceTestBase
{
    [Fact]
    public void MovieEndCutoff_UsesGivenGraceDays()
    {
        var now = new DateTime(2026, 11, 6, 0, 0, 0, DateTimeKind.Utc);

        ShowtimeLifecycle.MovieEndCutoff(now, 3).ShouldBe(now.AddDays(-3));
        ShowtimeLifecycle.MovieEndCutoff(now).ShouldBe(now.AddDays(-1));
    }

    [Fact]
    public async Task LoadShowtimeLookaheadDaysAsync_ReadsSettingsAndFallsBackToDefault()
    {
        (await Db.LoadShowtimeLookaheadDaysAsync(default)).ShouldBe(
            CinemaSettings.DefaultShowtimeLookaheadDays
        );

        var settings = await Db.CinemaSettings.SingleAsync(s => s.Id == 1);
        settings.ShowtimeLookaheadDays = 14;
        await Db.SaveChangesAsync();
        (await Db.LoadShowtimeLookaheadDaysAsync(default)).ShouldBe(14);

        Db.CinemaSettings.RemoveRange(Db.CinemaSettings);
        await Db.SaveChangesAsync();
        (await Db.LoadShowtimeLookaheadDaysAsync(default)).ShouldBe(
            CinemaSettings.DefaultShowtimeLookaheadDays
        );
    }
}

public class SettingsPolicyUpdateTests : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    private static UpdateSettingsRequest Request(
        int? roomBufferMinutes = null,
        int? maxSeatsPerBooking = null,
        int? schedulerStepMinutes = null,
        double? salesWeight = null,
        double? ratingWeight = null
    ) =>
        new(
            "Perfo Cinema",
            "1 Nguyen Hue",
            null,
            null,
            null,
            null,
            "09:00",
            "23:30",
            10_000m,
            0m,
            2,
            RoomBufferMinutes: roomBufferMinutes,
            MaxSeatsPerBooking: maxSeatsPerBooking,
            SchedulerStepMinutes: schedulerStepMinutes,
            SchedulerHeatSalesWeight: salesWeight,
            SchedulerHeatRatingWeight: ratingWeight
        );

    private SettingsService CreateSut()
    {
        _handle.Db.ChangeTracker.Clear();
        return new SettingsService(_handle.Db, Microsoft.Extensions.Options.Options.Create(new Cinema.Application.Options.BrandOptions()));
    }

    [Fact]
    public async Task UpdateSettingsAsync_PolicyFieldsProvided_ArePersisted()
    {
        var result = await CreateSut()
            .UpdateSettingsAsync(
                Request(roomBufferMinutes: 25, maxSeatsPerBooking: 6, schedulerStepMinutes: 10),
                CancellationToken.None
            );

        result.RoomBufferMinutes.ShouldBe(25);
        result.MaxSeatsPerBooking.ShouldBe(6);
        result.SchedulerStepMinutes.ShouldBe(10);
        var saved = await _handle.Db.CinemaSettings.AsNoTracking().SingleAsync(s => s.Id == 1);
        saved.RoomBufferMinutes.ShouldBe(25);
    }

    [Fact]
    public async Task UpdateSettingsAsync_PolicyFieldsOmitted_KeepExistingValues()
    {
        await CreateSut()
            .UpdateSettingsAsync(Request(roomBufferMinutes: 30), CancellationToken.None);

        var result = await CreateSut().UpdateSettingsAsync(Request(), CancellationToken.None);

        result.RoomBufferMinutes.ShouldBe(30);
        result.MaxComboQuantity.ShouldBe(CinemaSettings.DefaultMaxComboQuantity);
    }

    [Fact]
    public async Task UpdateSettingsAsync_BothHeatWeightsZero_ThrowsBusinessRule()
    {
        await Should.ThrowAsync<BusinessRuleException>(() =>
            CreateSut()
                .UpdateSettingsAsync(
                    Request(salesWeight: 0, ratingWeight: 0),
                    CancellationToken.None
                )
        );
    }

    [Fact]
    public void Validator_OutOfRangePolicyValues_AreRejected()
    {
        var validator = new UpdateSettingsRequestValidator();

        validator.Validate(Request(maxSeatsPerBooking: 0)).IsValid.ShouldBeFalse();
        validator.Validate(Request(roomBufferMinutes: -1)).IsValid.ShouldBeFalse();
        validator.Validate(Request(schedulerStepMinutes: 0)).IsValid.ShouldBeFalse();
        validator.Validate(Request(salesWeight: 0, ratingWeight: 0)).IsValid.ShouldBeFalse();
        validator.Validate(Request(roomBufferMinutes: 20, maxSeatsPerBooking: 8)).IsValid.ShouldBeTrue();
    }

    public void Dispose() => _handle.Dispose();
}
