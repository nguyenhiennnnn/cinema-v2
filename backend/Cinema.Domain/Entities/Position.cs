namespace Cinema.Domain.Entities;

public class Position
{
    public int PositionId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Employee> Employees { get; set; } = [];
}
