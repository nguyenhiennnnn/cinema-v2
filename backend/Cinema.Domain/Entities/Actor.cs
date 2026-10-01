namespace Cinema.Domain.Entities;

public class Actor
{
    public int ActorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }

    public ICollection<MovieCast> MovieCast { get; set; } = [];
}
