using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class Showtime
{
    public int ShowtimeId { get; set; }
    public int MovieId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal BasePrice { get; set; }
    public ShowtimeStatus Status { get; set; } = ShowtimeStatus.Scheduled;
    public DateTime CreatedAt { get; set; }

    public Movie Movie { get; set; } = null!;
    public CinemaRoom Room { get; set; } = null!;
    public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}
