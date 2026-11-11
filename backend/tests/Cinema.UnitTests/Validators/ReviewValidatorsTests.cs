using Cinema.Application.DTOs.Reviews;
using Cinema.Application.Validators.Reviews;
using Cinema.Shared.Resources;
using FluentValidation.TestHelper;
using Shouldly;

namespace Cinema.UnitTests.Validators;

public class ReviewValidatorsTests
{
    private readonly CreateReviewRequestValidator _validator = new();

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void ValidRating_PassesValidation(int rating)
    {
        var request = new CreateReviewRequest(rating, "Phim hay");
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void RatingOutOf1To5Range_FailsValidation(int rating)
    {
        var request = new CreateReviewRequest(rating, "Phim hay");
        var result = _validator.TestValidate(request);
        result
            .ShouldHaveValidationErrorFor(x => x.Rating)
            .WithErrorMessage(Messages.Validation_Rating_Range1To5);
    }

    [Fact]
    public void CommentExceeding1000Chars_FailsValidation()
    {
        var request = new CreateReviewRequest(5, new string('a', 1001));
        var result = _validator.TestValidate(request);
        result
            .ShouldHaveValidationErrorFor(x => x.Comment)
            .WithErrorMessage(Messages.Validation_ReviewContent_MaxLength1000);
    }
}
