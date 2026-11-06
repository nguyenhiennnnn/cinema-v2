namespace Cinema.Application.Options;

public class BookingOptions
{
    public const string SectionName = "Booking";

    // RoomBufferMinutes / MaxSeatsPerBooking / MaxComboQuantity đã chuyển sang CinemaSettings (DB).
    public int CustomerLookupLimit { get; set; } = 20;
    public int BulkDeleteLimit { get; set; } = 50;

    public int BookingCodeMaxAttempts { get; set; } = 5;

    public int DefaultPageSize { get; set; } = 20;

    public int MaxPageSize { get; set; } = 100;
}
