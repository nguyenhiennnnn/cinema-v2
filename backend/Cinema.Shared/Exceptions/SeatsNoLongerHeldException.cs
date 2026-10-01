namespace Cinema.Shared.Exceptions;

public class SeatsNoLongerHeldException(int bookingId)
    : BusinessRuleException(
        $"Booking {bookingId}: ghế giữ đã hết hạn trước khi xác nhận thanh toán."
    )
{
    public int BookingId { get; } = bookingId;
}
