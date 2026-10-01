namespace Cinema.Domain.Entities;

public class MembershipTier
{
    public int TierId { get; set; }
    public string TierName { get; set; } = null!;
    public int MinPoints { get; set; }
    public decimal DiscountPercent { get; set; }

    public ICollection<CustomerProfile> CustomerProfiles { get; set; } = [];
}
