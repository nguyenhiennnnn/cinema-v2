using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Avatar { get; set; }
    public string? GoogleId { get; set; }
    public int RoleId { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Role Role { get; set; } = null!;
    public CustomerProfile? CustomerProfile { get; set; }
    public Employee? Employee { get; set; }
}
