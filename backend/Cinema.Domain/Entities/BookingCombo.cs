namespace Cinema.Domain.Entities;

public class BookingCombo
{
    public int BookingComboId { get; set; }
    public int BookingId { get; set; }
    public int ComboId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }

    public Booking Booking { get; set; } = null!;
    public Combo Combo { get; set; } = null!;
}
