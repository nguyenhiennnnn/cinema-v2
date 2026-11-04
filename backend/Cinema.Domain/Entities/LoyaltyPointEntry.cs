namespace Cinema.Domain.Entities;

public class LoyaltyPointEntry
{
    public int EntryId { get; set; }
    public int UserId { get; set; }
    public int? BookingId { get; set; }
    public int Points { get; set; }
    public int RevokedPoints { get; set; }
    public DateTime EarnedAt { get; set; }

    public User User { get; set; } = null!;
    public Booking? Booking { get; set; }
}
