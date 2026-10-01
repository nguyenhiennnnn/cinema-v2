using Cinema.Domain.Enums;

namespace Cinema.Domain.Entities;

public class Payment
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionId { get; set; }

    public string? GatewayTransactionId { get; set; }
    public string? GatewayResponseCode { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? RefundTransactionId { get; set; }
    public decimal? RefundedAmount { get; set; }
    public DateTime? RefundedAt { get; set; }

    public decimal? AmountReceived { get; set; }
    public decimal? ChangeGiven { get; set; }

    public Booking Booking { get; set; } = null!;
}
