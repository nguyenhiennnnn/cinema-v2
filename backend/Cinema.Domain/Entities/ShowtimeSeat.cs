using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class ShowtimeSeat
{
    public int ShowtimeSeatId { get; set; }
    public int ShowtimeId { get; set; }
    public int SeatId { get; set; }
    public ShowtimeSeatStatus Status { get; set; } = ShowtimeSeatStatus.Available;
    public int? HeldByUserId { get; set; }
    public DateTime? HeldUntil { get; set; }
    public decimal Price { get; set; }

    public Showtime Showtime { get; set; } = null!;
    public Seat Seat { get; set; } = null!;
    public User? HeldByUser { get; set; }
    public BookingTicket? BookingTicket { get; set; }
}
