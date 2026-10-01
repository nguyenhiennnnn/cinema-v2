namespace Cinema.Application.DTOs.Payments;

public record AdminPaymentListItemDto(
    int PaymentId,
    int BookingId,
    string BookingCode,
    string PaymentMethod,
    string? TransactionId,
    string? GatewayResponseCode,
    decimal Amount,
    string Status,
    DateTime? PaidAt,
    DateTime CreatedAt
);

public record AdminPaymentBookingBriefDto(
    int BookingId,
    string BookingCode,
    string Status,
    decimal FinalAmount
);

public record AdminPaymentDetailDto(
    int PaymentId,
    int BookingId,
    string PaymentMethod,
    string? TransactionId,
    string? GatewayResponseCode,
    decimal Amount,
    string Status,
    DateTime? PaidAt,
    DateTime CreatedAt,
    decimal? AmountReceived,
    decimal? ChangeGiven,
    string? RefundTransactionId,
    decimal? RefundedAmount,
    DateTime? RefundedAt,
    AdminPaymentBookingBriefDto Booking
);
