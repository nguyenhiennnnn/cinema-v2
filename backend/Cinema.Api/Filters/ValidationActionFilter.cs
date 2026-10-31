using Cinema.Shared.Resources;
using Cinema.Shared.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cinema.Api.Filters;

public class ValidationActionFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (serviceProvider.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted
            );

            if (!result.IsValid)
            {
                var errors = result
                    .Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => char.ToLowerInvariant(g.Key[0]) + g.Key[1..],
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                context.Result = new BadRequestObjectResult(
                    ApiResponse.ErrorResponse(Messages.Common_InvalidData, errors)
                );
                return;
            }
        }

        await next();
    }
}
