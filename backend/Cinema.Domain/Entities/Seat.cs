namespace Cinema.Domain.Entities;

public class Seat
{
    public int SeatId { get; set; }
    public int RoomId { get; set; }
    public string SeatRow { get; set; } = null!;
    public int SeatNumber { get; set; }
    public int? SeatTypeId { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsAisle { get; set; } = false;

    public Guid? CoupleId { get; set; }

    public CinemaRoom Room { get; set; } = null!;
    public SeatType? SeatType { get; set; }
    public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = [];
}
