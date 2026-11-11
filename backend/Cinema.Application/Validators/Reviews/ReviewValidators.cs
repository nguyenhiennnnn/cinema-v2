using Cinema.Application.DTOs.Reviews;
using Cinema.Application.Options;
using Cinema.Shared.Resources;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Cinema.Application.Validators.Reviews;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage(Messages.Validation_Rating_Range1To5);

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage(Messages.Validation_ReviewContent_MaxLength1000);
    }
}

public class BulkDeleteReviewsRequestValidator : AbstractValidator<BulkDeleteReviewsRequest>
{
    public BulkDeleteReviewsRequestValidator(IOptions<BookingOptions> options)
    {
        var limit = options.Value.BulkDeleteLimit;
        RuleFor(x => x.ReviewIds).NotEmpty().WithMessage(Messages.Validation_ReviewIds_NotEmpty);
        RuleFor(x => x.ReviewIds)
            .Must(ids => ids.Count <= limit)
            .WithMessage($"T\u1ed1i \u0111a {{limit}} \u0111\u00e1nh gi\u00e1 m\u1ed7i l\u1ea7n.");
        RuleForEach(x => x.ReviewIds)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_ReviewId_Positive);
    }
}
