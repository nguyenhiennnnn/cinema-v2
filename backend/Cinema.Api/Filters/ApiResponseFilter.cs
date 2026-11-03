using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cinema.Api.Filters;

public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is not null)
            return;

        var metadata = context.ActionDescriptor.EndpointMetadata;
        var hasRawAttribute = metadata.Any(m => m is RawResponseAttribute);
        if (hasRawAttribute)
            return;

        var message = ResolveSuccessMessage(metadata);

        if (context.Result is ObjectResult objectResult && objectResult.Value is not ApiResponse)
        {
            var isSuccessStatus = objectResult.StatusCode is null or < 400;
            objectResult.Value = ApiResponse.SuccessResponse(
                objectResult.Value,
                isSuccessStatus ? message : null
            );
        }
        else if (context.Result is EmptyResult)
        {
            context.Result = new ObjectResult(ApiResponse.SuccessResponse(null, message))
            {
                StatusCode = 200,
            };
        }
    }

    private static string? ResolveSuccessMessage(IList<object> metadata)
    {
        var attribute = metadata.OfType<SuccessMessageAttribute>().FirstOrDefault();
        return attribute is null
            ? null
            : Messages.ResourceManager.GetString(attribute.ResourceKey, Messages.Culture);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class RawResponseAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class SuccessMessageAttribute(string resourceKey) : Attribute
{
    public string ResourceKey { get; } = resourceKey;
}
