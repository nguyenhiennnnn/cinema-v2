namespace Cinema.Domain.Entities;

public class ScreenType
{
    public int ScreenTypeId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<CinemaRoom> Rooms { get; set; } = [];
}
