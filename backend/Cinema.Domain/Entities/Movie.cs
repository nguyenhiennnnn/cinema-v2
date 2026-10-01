using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class Movie
{
    public int MovieId { get; set; }
    public string Title { get; set; } = null!;
    public string? OriginalTitle { get; set; }
    public string? Description { get; set; }
    public int? DirectorId { get; set; }
    public int DurationMinutes { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public int? LanguageId { get; set; }
    public string? Subtitle { get; set; }
    public int? AgeRatingId { get; set; }
    public string? PosterUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public MovieStatus Status { get; set; } = MovieStatus.ComingSoon;
    public decimal AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }

    public Director? Director { get; set; }
    public Language? Language { get; set; }
    public AgeRating? AgeRating { get; set; }
    public ICollection<MovieGenre> MovieGenres { get; set; } = [];
    public ICollection<MovieCast> MovieCast { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Showtime> Showtimes { get; set; } = [];
}
