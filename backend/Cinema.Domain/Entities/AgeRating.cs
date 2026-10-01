namespace Cinema.Domain.Entities;

public class AgeRating
{
    public int AgeRatingId { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<Movie> Movies { get; set; } = [];
}
