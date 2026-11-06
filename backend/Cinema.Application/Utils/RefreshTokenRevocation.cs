using Cinema.Application.Interfaces;
using Cinema.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Application.Utils;

/// <summary>
/// Điểm duy nhất thu hồi refresh token. Các hàm KHÔNG gọi SaveChanges — caller tự lưu
/// để thu hồi nằm chung transaction/lần lưu với thay đổi nghiệp vụ.
/// </summary>
public static class RefreshTokenRevocation
{
    /// <summary>Đánh dấu thu hồi một token. Trả về false nếu token đã bị thu hồi từ trước.</summary>
    public static bool Revoke(this RefreshToken token, DateTime? now = null)
    {
        if (token.IsRevoked)
            return false;

        token.IsRevoked = true;
        token.RevokedAt = now ?? DateTime.UtcNow;
        return true;
    }

    /// <summary>Thu hồi mọi refresh token còn hiệu lực của một user. Trả về số token đã thu hồi.</summary>
    public static Task<int> RevokeActiveTokensAsync(
        this IAppDbContext db,
        int userId,
        CancellationToken cancellationToken,
        DateTime? now = null
    ) => db.RevokeActiveTokensAsync([userId], cancellationToken, now);

    /// <summary>Thu hồi mọi refresh token còn hiệu lực của nhiều user. Trả về số token đã thu hồi.</summary>
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
