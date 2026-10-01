namespace Cinema.Domain.Entities;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public int UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public int? ReplacedByTokenId { get; set; }

    public User User { get; set; } = null!;
    public RefreshToken? ReplacedByToken { get; set; }
}
