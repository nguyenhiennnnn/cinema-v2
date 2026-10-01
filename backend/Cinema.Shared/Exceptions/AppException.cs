namespace Cinema.Shared.Exceptions;

public abstract class AppException : Exception
{
    public int StatusCode { get; }
    public IDictionary<string, string[]>? Errors { get; }

    protected AppException(
        int statusCode,
        string message,
        IDictionary<string, string[]>? errors = null
    )
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}
