using Cinema.Application.Interfaces;
using Cinema.Application.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinema.Application.Services.Bookings;

public class LoyaltyExpiryService(IAppDbContext db, ILogger<LoyaltyExpiryService> logger)
{
    public async Task<int> RecalculateAllAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var cutoff = LoyaltyWindow.Cutoff(now, await LoyaltyWindow.LoadMonthsAsync(db, ct));

        var sums = await db
            .LoyaltyPointEntries.Where(e => e.EarnedAt > cutoff)
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, Points = g.Sum(e => e.Points - e.RevokedPoints) })
            .ToDictionaryAsync(x => x.UserId, x => x.Points, ct);

        var tiers = await db.MembershipTiers.OrderByDescending(t => t.MinPoints).ToListAsync(ct);
        var profiles = await db
            .CustomerProfiles.Where(p => p.Points > 0 || p.TierId != null)
            .ToListAsync(ct);

        var changed = 0;
        foreach (var p in profiles)
        {
            var expected = sums.GetValueOrDefault(p.UserId);
            var tierId = tiers.FirstOrDefault(t => t.MinPoints <= expected)?.TierId;
            if (p.Points == expected && p.TierId == tierId)
                continue;
            p.Points = expected;
            p.TierId = tierId;
            changed++;
        }

        if (changed > 0)
            await db.SaveChangesAsync(ct);
        logger.LogInformation("LoyaltyExpiry recalculated {Changed} profiles", changed);
        return changed;
    }
}
