namespace Cinema.Domain.Entities;

public class Combo
{
    public int ComboId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<BookingCombo> BookingCombos { get; set; } = [];
}
