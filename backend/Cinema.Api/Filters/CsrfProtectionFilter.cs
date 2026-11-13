using Cinema.Api.Extensions;
using Cinema.Application.Options;
using Cinema.Shared.Exceptions;
using Cinema.Shared.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Cinema.Api.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class CsrfProtectionAttribute : TypeFilterAttribute
{
    public CsrfProtectionAttribute()
        : base(typeof(CsrfProtectionFilter))
    {
        Order = int.MinValue;
    }
}

public class CsrfProtectionFilter : IAsyncActionFilter
{
    public const string HeaderName = "X-Requested-With";
    public const string HeaderValue = "XMLHttpRequest";

    private readonly HashSet<string> _allowedOrigins;

    public CsrfProtectionFilter(IConfiguration configuration, IOptions<AppUrlOptions> appUrl)
    {
        _allowedOrigins = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var origin in configuration.GetCorsOrigins().Append(appUrl.Value.FrontendBaseUrl))
        {
            var normalized = NormalizeOrigin(origin);
            if (normalized is not null)
                _allowedOrigins.Add(normalized);
        }
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        if (
            !request.Headers.TryGetValue(HeaderName, out var marker)
            || !string.Equals(marker.ToString(), HeaderValue, StringComparison.Ordinal)
        )
            throw new ForbiddenAppException(Messages.Forbidden_CsrfRejected);

        if (request.Headers.TryGetValue("Origin", out var originHeader))
        {
            var origin = NormalizeOrigin(originHeader.ToString());
            if (origin is null || !_allowedOrigins.Contains(origin))
                throw new ForbiddenAppException(Messages.Forbidden_CsrfRejected);
        }

        return next();
    }

    private static string? NormalizeOrigin(string? value) =>
        Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            ? uri.GetLeftPart(UriPartial.Authority)
            : null;
}
