using Cinema.Application.DTOs.Reviews;
using Cinema.Application.Options;
using Microsoft.Extensions.Options;
using FluentValidation;

namespace Cinema.Application.Validators.Reviews;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 10)
            .WithMessage("Điểm đánh giá (rating) phải từ 1 đến 10.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage("Nội dung đánh giá không được vượt quá 1000 ký tự.");
    }
}

public class BulkDeleteReviewsRequestValidator : AbstractValidator<BulkDeleteReviewsRequest>
{
    public BulkDeleteReviewsRequestValidator(IOptions<BookingOptions> options)
    {
        var limit = options.Value.BulkDeleteLimit;
        RuleFor(x => x.ReviewIds)
            .NotEmpty()
            .WithMessage("Danh s\u00e1ch reviewIds kh\u00f4ng \u0111\u01b0\u1ee3c r\u1ed7ng.");
        RuleFor(x => x.ReviewIds)
            .Must(ids => ids.Count <= limit)
            .WithMessage($"T\u1ed1i \u0111a {{limit}} \u0111\u00e1nh gi\u00e1 m\u1ed7i l\u1ea7n.");
        RuleForEach(x => x.ReviewIds)
            .GreaterThan(0)
            .WithMessage("reviewId ph\u1ea3i l\u1edbn h\u01a1n 0.");
    }
}
