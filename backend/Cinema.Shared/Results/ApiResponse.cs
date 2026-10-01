namespace Cinema.Shared.Results;

public class ApiResponse
{
    public bool Success { get; init; }
    public object? Data { get; init; }
    public string? Message { get; init; }
    public IDictionary<string, string[]>? Errors { get; init; }

    public static ApiResponse SuccessResponse(object? data, string? message = null) =>
        new()
        {
            Success = true,
            Data = data,
            Message = message,
        };

    public static ApiResponse ErrorResponse(
        string message,
        IDictionary<string, string[]>? errors = null
    ) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors,
        };
}
