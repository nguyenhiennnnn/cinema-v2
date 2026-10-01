namespace Cinema.Domain.Entities;

public class MovieCast
{
    public int MovieId { get; set; }
    public int ActorId { get; set; }
    public string? CharacterName { get; set; }

    public Movie Movie { get; set; } = null!;
    public Actor Actor { get; set; } = null!;
}
