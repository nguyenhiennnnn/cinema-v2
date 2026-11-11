namespace Cinema.Application.DTOs.Bookings;

public record ComboSelectionRequest(int ComboId, int Quantity);

public record HoldSeatsRequest(
    int ShowtimeId,
    string CustomerName,
    string? CustomerPhone,
    List<int> SeatIds,
    List<ComboSelectionRequest>? Combos
);

public record HoldBookingResponse(
    int BookingId,
    string BookingCode,
    string Status,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    DateTime ExpiresAt
);

public record BookingListItemDto(
    int BookingId,
    string BookingCode,
    string MovieTitle,
    DateTime StartTime,
    decimal FinalAmount,
    string Status,
    DateTime CreatedAt
);

public record BookingDetailDto(
    int BookingId,
    string BookingCode,
    string Status,
    BookingMovieBriefDto Movie,
    BookingShowtimeBriefDto Showtime,
    List<BookingSeatDto> Seats,
    List<BookingComboItemDto> Combos,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    DateTime CreatedAt,
    List<BookingTicketDto> Tickets,
    List<BookingPaymentDto> Payments,
    DateTime? HoldExpiresAt
);

public record CheckoutRequest(string PaymentMethod);

public record CheckoutResponse(string PaymentUrl, int PaymentId);

public record CancelBookingResponse(
    int BookingId,
    string Status,
    decimal? RefundAmount,
    string? RefundStatus
);
