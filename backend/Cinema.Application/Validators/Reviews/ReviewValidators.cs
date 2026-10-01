using Cinema.Application.DTOs.Reviews;
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
