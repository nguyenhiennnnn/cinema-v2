namespace Cinema.Shared.Exceptions;

public class ConflictAppException : AppException
{
    public ConflictAppException(string message)
        : base(StatusCodes.Status409Conflict, message) { }
}
