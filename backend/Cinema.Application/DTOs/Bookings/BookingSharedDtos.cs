namespace Cinema.Application.DTOs.Bookings;

public record BookingMovieBriefDto(int MovieId, string Title, string? PosterUrl);

public record BookingShowtimeBriefDto(int ShowtimeId, string RoomName, DateTime StartTime);

public record BookingSeatDto(string SeatRow, int SeatNumber, decimal Price);

public record BookingComboItemDto(string Name, int Quantity, decimal UnitPrice);

public record BookingTicketDto(
    int TicketId,
    string SeatRow,
    int SeatNumber,
    string QrCodeImage,
    bool IsCheckedIn,
    DateTime? CheckedInAt
);

public record BookingPaymentDto(
    int PaymentId,
    string PaymentMethod,
    string Status,
    decimal Amount,
    string? TransactionId,
    DateTime? PaidAt,
    DateTime CreatedAt,
    decimal? RefundedAmount,
    DateTime? RefundedAt
);
