namespace Cinema.Application.DTOs.Bookings;

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
