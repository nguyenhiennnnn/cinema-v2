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

        var hasRawAttribute = context.ActionDescriptor.EndpointMetadata.Any(m =>
            m is RawResponseAttribute
        );
        if (hasRawAttribute)
            return;

        if (context.Result is ObjectResult objectResult && objectResult.Value is not ApiResponse)
        {
            objectResult.Value = ApiResponse.SuccessResponse(objectResult.Value);
        }
        else if (context.Result is EmptyResult)
        {
            context.Result = new ObjectResult(ApiResponse.SuccessResponse(null))
            {
                StatusCode = 200,
            };
        }
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class RawResponseAttribute : Attribute;
