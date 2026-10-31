using Cinema.Shared.Resources;

namespace Cinema.Shared.Exceptions;

public class SeatsNoLongerHeldException(int bookingId)
    : BusinessRuleException(
        string.Format(Messages.Rule_SeatsNoLongerHeld, bookingId)
    )
{
    public int BookingId { get; } = bookingId;
}
