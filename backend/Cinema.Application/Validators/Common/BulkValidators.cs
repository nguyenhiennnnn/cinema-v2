using System.Linq.Expressions;
using Cinema.Application.DTOs.Common;
using Cinema.Application.Options;
using Cinema.Shared.Resources;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Validators.Common;

internal static class BulkIdRules
{
    public static void Apply<T>(
        AbstractValidator<T> validator,
        Expression<Func<T, IEnumerable<int>>> selector,
        int limit
    )
    {
        validator
            .RuleFor(selector)
            .NotEmpty()
            .WithMessage(Messages.Validation_BulkIds_NotEmpty);
            
        validator
            .RuleFor(selector)
            .Must(ids => ids != null && ids.Distinct().Count() <= limit)
            .WithMessage(string.Format(Messages.Validation_BulkIds_MaxCount, limit));
            
        validator
            .RuleForEach(selector)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_BulkId_Positive);
    }
}