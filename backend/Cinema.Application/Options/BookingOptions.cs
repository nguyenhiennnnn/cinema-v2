namespace Cinema.Application.Options;

public class BookingOptions
{
    public const string SectionName = "Booking";

    public int HoldDurationMinutes { get; set; } = 15;
    public int RoomBufferMinutes { get; set; } = 15;
    public int CheckInOpenMinutes { get; set; } = 30;
    public int MaxSeatsPerBooking { get; set; } = 10;
    public int MaxComboQuantity { get; set; } = 10;
}
