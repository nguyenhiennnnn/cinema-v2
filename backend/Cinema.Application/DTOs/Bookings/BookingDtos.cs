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

public record CounterHoldRequest(
    int ShowtimeId,
    List<int> SeatIds,
    List<ComboSelectionRequest>? Combos,
    int? UserId,
    string CustomerName,
    string? CustomerPhone
);

public record CounterHoldResponse(
    int BookingId,
    string BookingCode,
    string Status,
    decimal TotalAmount,
    decimal DiscountAmount,
    decimal FinalAmount,
    DateTime ExpiresAt,
    string Channel,
    int? CreatedByEmployeeId
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

public record BookingEmployeeBriefDto(int EmployeeId, string FullName);

public record AdminBookingListItemDto(
    int BookingId,
    string BookingCode,
    string CustomerName,
    string? GuestPhone,
    decimal FinalAmount,
    string Status,
    string Channel,
    DateTime CreatedAt
);

public record AdminBookingDetailDto(
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
    string Channel,
    BookingEmployeeBriefDto? CreatedByEmployee,
    List<BookingTicketDto> Tickets,
    List<BookingPaymentDto> Payments
);

public record CheckoutRequest(string PaymentMethod);

public record CheckoutResponse(string PaymentUrl, int PaymentId);

public record CancelBookingResponse(
    int BookingId,
    string Status,
    decimal? RefundAmount,
    string? RefundStatus
);

public record CounterCheckoutRequest(decimal AmountReceived);

public record CounterCheckoutResponse(int BookingId, string Status, decimal Change);

public record CheckinRequest(string QrCode);

public record CheckinResponse(
    int TicketId,
    int BookingId,
    string SeatRow,
    int SeatNumber,
    string MovieTitle,
    DateTime StartTime,
    DateTime CheckedInAt
);
