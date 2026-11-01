using Cinema.Application.DTOs.Settings;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Settings;

public class SettingsService(IAppDbContext db) : ISettingsService
{
    public async Task<CinemaSettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var settings =
            await db.CinemaSettings.FirstOrDefaultAsync(s => s.Id == AppDbContextExtensions.CinemaSettingsId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_CinemaSettings);

        return settings.Adapt<CinemaSettingsDto>();
    }

    public async Task<CinemaSettingsDto> UpdateSettingsAsync(
        UpdateSettingsRequest request,
        CancellationToken cancellationToken
    )
    {
        var settings =
            await db.CinemaSettings.FirstOrDefaultAsync(s => s.Id == AppDbContextExtensions.CinemaSettingsId, cancellationToken)
            ?? throw new NotFoundException(Messages.NotFound_CinemaSettings);

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
        settings.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return settings.Adapt<CinemaSettingsDto>();
    }
}
