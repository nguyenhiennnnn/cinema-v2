namespace Cinema.Domain.Entities;

public class Director
{
    public int DirectorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }

    public ICollection<Movie> Movies { get; set; } = [];
}
