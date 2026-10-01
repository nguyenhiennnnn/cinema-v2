using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class UserVerificationToken
{
    public int TokenId { get; set; }
    public int UserId { get; set; }
    public string TokenHash { get; set; } = null!;
    public VerificationTokenPurpose Purpose { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
