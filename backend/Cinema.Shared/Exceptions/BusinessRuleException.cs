namespace Cinema.Shared.Exceptions;

public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message)
        : base(StatusCodes.Status422UnprocessableEntity, message) { }
}
