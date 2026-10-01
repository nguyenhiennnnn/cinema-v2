namespace Cinema.Shared.Exceptions;

public class ValidationAppException : AppException
{
    public ValidationAppException(string message, IDictionary<string, string[]>? errors = null)
        : base(StatusCodes.Status400BadRequest, message, errors) { }
}
