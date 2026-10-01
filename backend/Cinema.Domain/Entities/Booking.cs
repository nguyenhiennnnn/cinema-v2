using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class Booking
{
    public int BookingId { get; set; }
    public int? UserId { get; set; }
    public int ShowtimeId { get; set; }
    public string BookingCode { get; set; } = null!;
    public BookingChannel Channel { get; set; } = BookingChannel.Online;
    public int? CreatedByEmployeeId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string? CustomerPhone { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public User? User { get; set; }
    public Showtime Showtime { get; set; } = null!;
    public Employee? CreatedByEmployee { get; set; }
    public ICollection<BookingTicket> Tickets { get; set; } = [];
    public ICollection<BookingCombo> BookingCombos { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
}
