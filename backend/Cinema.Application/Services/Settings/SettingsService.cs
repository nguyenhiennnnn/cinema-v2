using Cinema.Application.DTOs.Settings;
using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Cinema.Shared.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Services.Settings;

public class SettingsService(IAppDbContext db) : ISettingsService
{
    public async Task<CinemaSettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var settings =
            await db.CinemaSettings.FirstOrDefaultAsync(s => s.Id == 1, cancellationToken)
            ?? throw new NotFoundException("Chưa có cấu hình rạp — liên hệ Admin khởi tạo.");

        return settings.Adapt<CinemaSettingsDto>();
    }

    public async Task<CinemaSettingsDto> UpdateSettingsAsync(
        UpdateSettingsRequest request,
        CancellationToken cancellationToken
    )
    {
        var settings =
            await db.CinemaSettings.FirstOrDefaultAsync(s => s.Id == 1, cancellationToken)
            ?? throw new NotFoundException("Chưa có cấu hình rạp — liên hệ Admin khởi tạo.");

        if (request.CancelRefundPercent is < 0 or > 100)
            throw new BusinessRuleException("CancelRefundPercent phải trong khoảng 0-100.");
        if (request.CancelDeadlineHours < 0)
            throw new BusinessRuleException("CancelDeadlineHours không được âm.");

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
        settings.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        return settings.Adapt<CinemaSettingsDto>();
    }
}
