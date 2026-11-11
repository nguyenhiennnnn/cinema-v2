using Cinema.Application.DTOs.Settings;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Entities;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Settings;

public class SettingsService(IAppDbContext db, IOptions<BrandOptions> brandOptions)
    : ISettingsService
{
    private readonly BrandOptions _brand = brandOptions.Value;

    public async Task<CinemaSettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var settings =
            await db.CinemaSettings.FirstOrDefaultAsync(
                s => s.Id == AppDbContextExtensions.CinemaSettingsId,
                cancellationToken
            ) ?? throw new NotFoundException(Messages.NotFound_CinemaSettings);

        return ToDto(settings);
    }

    public async Task<CinemaSettingsDto> UpdateSettingsAsync(
        UpdateSettingsRequest request,
        CancellationToken cancellationToken
    )
    {
        var settings =
            await db.CinemaSettings.FirstOrDefaultAsync(
                s => s.Id == AppDbContextExtensions.CinemaSettingsId,
                cancellationToken
            ) ?? throw new NotFoundException(Messages.NotFound_CinemaSettings);

        if (request.CancelRefundPercent is < 0 or > 100)
            throw new BusinessRuleException(Messages.Rule_CancelRefundPercentRange);
        if (request.CancelDeadlineHours < 0)
            throw new BusinessRuleException(Messages.Rule_CancelDeadlineHoursNegative);

        settings.Name = request.Name;
        settings.Address = request.Address;
        settings.Phone = request.Phone;
        settings.Email = request.Email;
        settings.TaxCode = request.TaxCode;
        settings.LogoUrl = request.LogoUrl;
        settings.OpeningTime = DateTimeUtils.ParseTimeOnly(request.OpeningTime);
        settings.ClosingTime = DateTimeUtils.ParseTimeOnly(request.ClosingTime);
        settings.PointsPerAmount = request.PointsPerAmount;
        settings.CancelRefundPercent = request.CancelRefundPercent;
        settings.CancelDeadlineHours = request.CancelDeadlineHours;
        settings.StopSellingMinutesBefore = request.StopSellingMinutesBefore;
        if (request.HoldDurationMinutes is { } holdDurationMinutes)
            settings.HoldDurationMinutes = holdDurationMinutes;
        if (request.CheckInOpenMinutes is { } checkInOpenMinutes)
            settings.CheckInOpenMinutes = checkInOpenMinutes;
        if (Enum.TryParse<SeatGapRuleMode>(request.SeatGapRuleMode, out var seatGapRuleMode))
            settings.SeatGapRuleMode = seatGapRuleMode;
        if (request.RoomBufferMinutes is { } roomBufferMinutes)
            settings.RoomBufferMinutes = roomBufferMinutes;
        if (request.MaxSeatsPerBooking is { } maxSeatsPerBooking)
            settings.MaxSeatsPerBooking = maxSeatsPerBooking;
        if (request.MaxComboQuantity is { } maxComboQuantity)
            settings.MaxComboQuantity = maxComboQuantity;
        if (request.LoyaltyWindowMonths is { } loyaltyWindowMonths)
            settings.LoyaltyWindowMonths = loyaltyWindowMonths;
        if (request.MovieEndGraceDays is { } movieEndGraceDays)
            settings.MovieEndGraceDays = movieEndGraceDays;
        if (request.ShowtimeLookaheadDays is { } showtimeLookaheadDays)
            settings.ShowtimeLookaheadDays = showtimeLookaheadDays;
        if (request.SchedulerStepMinutes is { } schedulerStepMinutes)
            settings.SchedulerStepMinutes = schedulerStepMinutes;
        if (request.SchedulerMinSameMovieGapMinutes is { } schedulerMinGap)
            settings.SchedulerMinSameMovieGapMinutes = schedulerMinGap;
        if (request.SchedulerHeatLookbackDays is { } schedulerLookback)
            settings.SchedulerHeatLookbackDays = schedulerLookback;
        if (request.SchedulerHeatSalesWeight is { } salesWeight)
            settings.SchedulerHeatSalesWeight = salesWeight;
        if (request.SchedulerHeatRatingWeight is { } ratingWeight)
            settings.SchedulerHeatRatingWeight = ratingWeight;
        if (request.SchedulerMaxRating is { } schedulerMaxRating)
            settings.SchedulerMaxRating = schedulerMaxRating;
        if (settings.SchedulerHeatSalesWeight + settings.SchedulerHeatRatingWeight <= 0)
            throw new BusinessRuleException(Messages.Validation_SchedulerHeatWeights_SumPositive);
        settings.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return ToDto(settings);
    }

    private CinemaSettingsDto ToDto(CinemaSettings settings) =>
        settings.Adapt<CinemaSettingsDto>() with
        {
            TimeZoneId = BusinessTime.IanaId,
            Locale = _brand.Locale,
            CurrencyCode = _brand.CurrencyCode,
            CurrencySymbol = _brand.CurrencySymbol,
        };
}
