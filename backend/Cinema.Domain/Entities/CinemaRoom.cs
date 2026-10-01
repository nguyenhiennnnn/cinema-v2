namespace Cinema.Domain.Entities;

public class CinemaRoom
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = null!;
    public int? ScreenTypeId { get; set; }
    public int TotalSeats { get; set; }
    public bool IsActive { get; set; } = true;

    public int Rows { get; set; } = 10;
    public int Cols { get; set; } = 10;

    public ScreenType? ScreenType { get; set; }
    public ICollection<Seat> Seats { get; set; } = [];
    public ICollection<Showtime> Showtimes { get; set; } = [];
}
