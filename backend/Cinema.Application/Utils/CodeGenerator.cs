namespace Cinema.Application.Utils;

public static class CodeGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public const string DefaultBookingCodePrefix = "BK";

    private static string _bookingCodePrefix = DefaultBookingCodePrefix;

    public static void Configure(string bookingCodePrefix)
    {
        if (!string.IsNullOrWhiteSpace(bookingCodePrefix))
            _bookingCodePrefix = bookingCodePrefix.Trim().ToUpperInvariant();
    }

    public static string GenerateBookingCode()
    {
        var suffix = new string(
            Enumerable.Range(0, 4).Select(_ => Chars[Random.Shared.Next(Chars.Length)]).ToArray()
        );

        return $"{_bookingCodePrefix}{VietnamTime.Today():yyyyMMdd}{suffix}";
    }

    public static string GenerateTicketQrPayload(
        string bookingCode,
        string seatRow,
        int seatNumber
    ) => $"{bookingCode}-{seatRow}{seatNumber}";

    public static string GeneratePaymentTransactionId(string bookingCode) =>
        $"{bookingCode}{DateTimeOffset.Now.ToUnixTimeSeconds()}";
}
