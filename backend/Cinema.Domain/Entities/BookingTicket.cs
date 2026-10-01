namespace Cinema.Domain.Entities;

public class BookingTicket
{
    public int TicketId { get; set; }
    public int BookingId { get; set; }
    public int ShowtimeSeatId { get; set; }
    public decimal Price { get; set; }
    public string? QrCode { get; set; }
    public bool IsCheckedIn { get; set; }
    public DateTime? CheckedInAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public ShowtimeSeat ShowtimeSeat { get; set; } = null!;
}
