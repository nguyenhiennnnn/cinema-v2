using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Utils;

public static class LoyaltyWindow
{
    public const int Months = 12;

    public static DateTime Cutoff(DateTime utcNow) => utcNow.AddMonths(-Months);

    public static int PointsFor(decimal finalAmount, decimal pointsPerAmount) =>
        (int)Math.Floor(finalAmount / pointsPerAmount);

    public static async Task<int> SumActiveAsync(
        IAppDbContext db,
        int userId,
        DateTime utcNow,
        CancellationToken ct
    )
    {
        var cutoff = Cutoff(utcNow);
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
