namespace Cinema.Shared.Exceptions;

public class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message)
        : base(StatusCodes.Status403Forbidden, message) { }
}
