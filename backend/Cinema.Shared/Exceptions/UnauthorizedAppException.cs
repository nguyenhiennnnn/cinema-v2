namespace Cinema.Shared.Exceptions;

public class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message)
        : base(StatusCodes.Status401Unauthorized, message) { }
}
