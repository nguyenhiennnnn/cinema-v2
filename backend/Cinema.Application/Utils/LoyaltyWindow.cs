using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Utils;

public static class LoyaltyWindow
{
    public const int DefaultMonths = CinemaSettings.DefaultLoyaltyWindowMonths;

    public static DateTime Cutoff(DateTime utcNow, int months = DefaultMonths) =>
        utcNow.AddMonths(-months);

    public static async Task<int> LoadMonthsAsync(IAppDbContext db, CancellationToken ct) =>
        await db
            .CinemaSettings.Where(s => s.Id == AppDbContextExtensions.CinemaSettingsId)
            .Select(s => (int?)s.LoyaltyWindowMonths)
            .FirstOrDefaultAsync(ct)
        ?? DefaultMonths;

    public static int PointsFor(decimal finalAmount, decimal pointsPerAmount) =>
        (int)Math.Floor(finalAmount / pointsPerAmount);

    public static async Task<int> SumActiveAsync(
        IAppDbContext db,
        int userId,
        DateTime utcNow,
        CancellationToken ct
    )
    {
        var cutoff = Cutoff(utcNow, await LoadMonthsAsync(db, ct));
        var entries = await db
            .LoyaltyPointEntries.Where(e => e.UserId == userId && e.EarnedAt > cutoff)
            .ToListAsync(ct);
        return entries.Sum(e => e.Points - e.RevokedPoints);
    }

    public static async Task<MembershipTier?> ResolveTierAsync(
        IAppDbContext db,
        int points,
        CancellationToken ct
    ) =>
        await db
            .MembershipTiers.Where(t => t.MinPoints <= points)
            .OrderByDescending(t => t.MinPoints)
            .FirstOrDefaultAsync(ct);

    public static async Task<(int Points, MembershipTier? Tier)> GetStandingAsync(
        IAppDbContext db,
        int userId,
        DateTime utcNow,
        CancellationToken ct
    )
    {
        var points = await SumActiveAsync(db, userId, utcNow, ct);
        return (points, await ResolveTierAsync(db, points, ct));
    }

    public static async Task<Dictionary<int, (int Points, MembershipTier? Tier)>> GetStandingsAsync(
        IAppDbContext db,
        IReadOnlyCollection<int> userIds,
        DateTime utcNow,
        CancellationToken ct
    )
    {
        var result = new Dictionary<int, (int Points, MembershipTier? Tier)>();
        if (userIds.Count == 0)
            return result;

        var cutoff = Cutoff(utcNow, await LoadMonthsAsync(db, ct));
        var sums = await db
            .LoyaltyPointEntries.Where(e => userIds.Contains(e.UserId) && e.EarnedAt > cutoff)
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, Points = g.Sum(e => e.Points - e.RevokedPoints) })
            .ToDictionaryAsync(x => x.UserId, x => x.Points, ct);

        var tiers = await db.MembershipTiers.OrderByDescending(t => t.MinPoints).ToListAsync(ct);
        foreach (var id in userIds)
        {
            var points = sums.GetValueOrDefault(id);
            result[id] = (points, tiers.FirstOrDefault(t => t.MinPoints <= points));
        }
        return result;
    }

    public static async Task ResyncProfileAsync(
        IAppDbContext db,
        CustomerProfile profile,
        int pendingNewPoints,
        DateTime utcNow,
        CancellationToken ct
    )
    {
        profile.Points = await SumActiveAsync(db, profile.UserId, utcNow, ct) + pendingNewPoints;
        profile.TierId = (await ResolveTierAsync(db, profile.Points, ct))?.TierId;
    }
}
