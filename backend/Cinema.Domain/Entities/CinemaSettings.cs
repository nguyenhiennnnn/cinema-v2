namespace Cinema.Domain.Entities;

public class CinemaSettings
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxCode { get; set; }
    public string? LogoUrl { get; set; }
    public TimeOnly? OpeningTime { get; set; }
    public TimeOnly? ClosingTime { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public decimal PointsPerAmount { get; set; } = 10000m;

    public decimal CancelRefundPercent { get; set; } = 100m;

    public int CancelDeadlineHours { get; set; } = 2;
}
