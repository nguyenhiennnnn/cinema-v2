namespace Cinema.Domain.Entities;

public class CustomerProfile
{
    public int UserId { get; set; }
    public int Points { get; set; }
    public int? TierId { get; set; }
    public decimal TotalSpent { get; set; }

    public User User { get; set; } = null!;
    public MembershipTier? Tier { get; set; }
}
