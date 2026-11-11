using System.Globalization;
using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Utils;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Services.Ai;

public class AiSystemPromptBuilder(IAppDbContext db, IOptions<BrandOptions> brandOptions)
{
    private const string FallbackOpeningTime = "08:00";
    private const string FallbackClosingTime = "23:30";

    private readonly BrandOptions _brand = brandOptions.Value;

    public async Task<string> BuildSystemPromptAsync(
        int? userId,
        CancellationToken cancellationToken
    )
    {
        var settings = await db.CinemaSettings.FirstOrDefaultAsync(
            s => s.Id == AppDbContextExtensions.CinemaSettingsId,
            cancellationToken
        );
        var cinemaName = settings?.Name ?? _brand.Name;
        var address = settings?.Address ?? Messages.Ai_Prompt_NotUpdated;
        var phone = settings?.Phone ?? Messages.Ai_Prompt_NotUpdated;
        var opening =
            settings?.OpeningTime?.ToString("HH:mm", CultureInfo.InvariantCulture)
            ?? FallbackOpeningTime;
        var closing =
            settings?.ClosingTime?.ToString("HH:mm", CultureInfo.InvariantCulture)
            ?? FallbackClosingTime;

        var now = DateTime.UtcNow;

        var movies = await db
            .Movies.Include(m => m.AgeRating)
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Where(m => m.Status == MovieStatus.NowShowing || m.Status == MovieStatus.ComingSoon)
            .ToListAsync(cancellationToken);

        var combos = await db.Combos.Where(c => c.IsActive).ToListAsync(cancellationToken);

        string userInfoStr = Messages.Ai_Prompt_Guest;
        if (userId.HasValue)
        {
            var user = await db
                .Users.Include(u => u.CustomerProfile)
                    .ThenInclude(cp => cp!.Tier)
                .FirstOrDefaultAsync(u => u.UserId == userId.Value, cancellationToken);
            if (user is not null)
            {
                var standing = await LoyaltyWindow.GetStandingAsync(
                    db,
                    user.UserId,
                    now,
                    cancellationToken
                );
                var points = standing.Points;
                var tier = standing.Tier?.TierName ?? Messages.Ai_Prompt_DefaultTier;
                userInfoStr = string.Format(
                    Messages.Ai_Prompt_UserInfo,
                    user.FullName,
                    user.Email,
                    points,
                    tier
                );
            }
        }

        var movieInfoLines = movies.Select(m =>
            string.Format(
                Messages.Ai_Prompt_MovieLine,
                m.MovieId,
                m.Title,
                m.Subtitle ?? Messages.Ai_Prompt_DefaultSubtitle,
                string.Join(", ", m.MovieGenres.Select(g => g.Genre.Name)),
                m.DurationMinutes,
                m.AgeRating?.Code ?? "P",
                m.Status == MovieStatus.NowShowing
                    ? Messages.Ai_Prompt_MovieNowShowing
                    : Messages.Ai_Prompt_MovieComingSoon
            )
        );

        var comboLines = combos.Select(c =>
            string.Format(Messages.Ai_Prompt_ComboLine, c.Name, c.Price, c.Description)
        );

        var loginRule = userId.HasValue
            ? Messages.Ai_Prompt_LoggedInRule
            : Messages.Ai_Prompt_GuestRule;

        return string.Format(
            CultureInfo.InvariantCulture,
            Messages.Ai_Prompt_SystemTemplate,
            cinemaName,
            BusinessTime.ToLocal(now),
            address,
            phone,
            opening,
            closing,
            userInfoStr,
            string.Join("\n", movieInfoLines),
            string.Join("\n", comboLines),
            _brand.Name,
            loginRule,
            BusinessTime.IanaId
        );
    }
}
