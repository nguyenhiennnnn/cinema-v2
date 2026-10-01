namespace Cinema.Shared.Exceptions;

public class SeatConflictException : AppException
{
    public List<int> ConflictSeatIds { get; }

    public SeatConflictException(string message, List<int> conflictSeatIds)
        : base(StatusCodes.Status409Conflict, message)
    {
        ConflictSeatIds = conflictSeatIds;
    }
}
