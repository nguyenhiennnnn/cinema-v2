namespace Cinema.Shared.Exceptions;

public class SeatConflictException : AppException
{
    public List<int> ConflictSeatIds { get; }

    public const string ErrorKey = "conflictSeatIds";

    public SeatConflictException(string message, List<int> conflictSeatIds)
        : base(
            StatusCodes.Status409Conflict,
            message,
            new Dictionary<string, string[]>
            {
                [ErrorKey] = conflictSeatIds.Select(id => id.ToString()).ToArray(),
            }
        )
    {
        ConflictSeatIds = conflictSeatIds;
    }
}
