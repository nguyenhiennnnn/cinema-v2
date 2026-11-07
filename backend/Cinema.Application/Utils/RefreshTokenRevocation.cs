using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Utils;

public static class RefreshTokenRevocation
{
    public static bool Revoke(this RefreshToken token, DateTime? now = null)
    {
        if (token.IsRevoked)
            return false;

        token.IsRevoked = true;
        token.RevokedAt = now ?? DateTime.UtcNow;
        return true;
    }

    public static Task<int> RevokeActiveTokensAsync(
        this IAppDbContext db,
        int userId,
        CancellationToken cancellationToken,
        DateTime? now = null
    ) => db.RevokeActiveTokensAsync([userId], cancellationToken, now);

    public static async Task<int> RevokeActiveTokensAsync(
        this IAppDbContext db,
        IReadOnlyCollection<int> userIds,
        CancellationToken cancellationToken,
        DateTime? now = null
    )
    {
        if (userIds.Count == 0)
            return 0;

        var timestamp = now ?? DateTime.UtcNow;
        var ids = userIds.ToArray();
        var activeTokens = await db
            .RefreshTokens.Where(rt => ids.Contains(rt.UserId) && !rt.IsRevoked)
            .ToListAsync(cancellationToken);
        foreach (var token in activeTokens)
            token.Revoke(timestamp);

        return activeTokens.Count;
    }
}
