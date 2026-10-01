namespace Cinema.Domain.Entities;

public class Employee
{
    public int EmployeeId { get; set; }
    public int UserId { get; set; }
    public int? PositionId { get; set; }
    public DateOnly? HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
    public Position? Position { get; set; }
    public ICollection<Booking> CounterBookings { get; set; } = [];
}
