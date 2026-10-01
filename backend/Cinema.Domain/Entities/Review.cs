namespace Cinema.Domain.Entities;

public class Review
{
    public int ReviewId { get; set; }
    public int MovieId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public Movie Movie { get; set; } = null!;
    public User User { get; set; } = null!;
}
