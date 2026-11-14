using System.Security.Claims;
using Cinema.Shared.Constants;
using Cinema.Shared.Resources;
using Cinema.Shared.Results;

namespace Cinema.Api.Middlewares;

public class PasswordChangeRequiredMiddleware(RequestDelegate next)
{
    private static readonly (string Method, string Path)[] AllowedEndpoints =
    [
        ("GET", "/api/v1/auth/me"),
        ("POST", "/api/v1/auth/refresh"),
        ("POST", "/api/v1/auth/logout"),
        ("POST", "/api/v1/users/me/change-password"),
    ];

    public Task InvokeAsync(HttpContext context)
    {
        if (!MustChangePassword(context.User) || IsAllowed(context.Request))
            return next(context);

        return ApiErrorWriter.WriteAsync(
            context,
            StatusCodes.Status403Forbidden,
            ApiResponse.ErrorResponse(
                Messages.Forbidden_PasswordChangeRequired,
                errorCode: ApiErrorCodes.PasswordChangeRequired
            )
        );
    }

    private static bool MustChangePassword(ClaimsPrincipal user) =>
        string.Equals(
            user.FindFirstValue(AppClaimTypes.MustChangePassword),
            "true",
            StringComparison.OrdinalIgnoreCase
        );

    private static bool IsAllowed(HttpRequest request)
    {
        if (HttpMethods.IsOptions(request.Method))
            return true;

        var path = (request.Path.Value ?? string.Empty).TrimEnd('/');
        foreach (var (method, allowedPath) in AllowedEndpoints)
        {
            if (
                string.Equals(request.Method, method, StringComparison.OrdinalIgnoreCase)
                && string.Equals(path, allowedPath, StringComparison.OrdinalIgnoreCase)
            )
                return true;
        }

        return false;
    }
}
