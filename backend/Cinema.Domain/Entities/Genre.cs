namespace Cinema.Domain.Entities;

public class Genre
{
    public int GenreId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<MovieGenre> MovieGenres { get; set; } = [];
}
