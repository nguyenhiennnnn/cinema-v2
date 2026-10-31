namespace Cinema.Application.Options;

public class BookingOptions
{
    public const string SectionName = "Booking";

    public int RoomBufferMinutes { get; set; } = 15;
    public int MaxSeatsPerBooking { get; set; } = 10;
    public int MaxComboQuantity { get; set; } = 10;
    public int CustomerLookupLimit { get; set; } = 20;
    public int BulkDeleteLimit { get; set; } = 50;

    public int BookingCodeMaxAttempts { get; set; } = 5;

    public int DefaultPageSize { get; set; } = 20;

    public int MaxPageSize { get; set; } = 100;
}
