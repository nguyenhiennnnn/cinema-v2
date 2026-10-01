namespace Cinema.Application.Utils;

public static class CodeGenerator
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateBookingCode()
    {
        var suffix = new string(
            Enumerable
                .Range(0, 4)
                .Select(_ => Chars[Random.Shared.Next(Chars.Length)])
                .ToArray()
        );

        return $"BK{DateTime.UtcNow:yyyyMMdd}{suffix}";
    }

    public static string GenerateTicketQrPayload(string bookingCode, string seatRow, int seatNumber) =>
        $"{bookingCode}-{seatRow}{seatNumber}";

    public static string GeneratePaymentTransactionId(string bookingCode) =>
        $"{bookingCode}{DateTimeOffset.Now.ToUnixTimeSeconds()}";
}
