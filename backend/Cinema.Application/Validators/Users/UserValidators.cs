using Cinema.Application.DTOs.Users;
using Cinema.Domain.Enums;
using Cinema.Application.Utils;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Users;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    private static readonly string[] ValidGenders = Enum.GetNames<Gender>();

    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone)
            .Matches(PhoneRules.Pattern)
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage(Messages.Validation_Phone_Invalid)
            .MaximumLength(20);
        RuleFor(x => x.Avatar).MaximumLength(255);
        RuleFor(x => x.Gender)
            .Must(g => ValidGenders.Contains(g))
            .When(x => x.Gender is not null)
            .WithMessage(string.Format(Messages.Validation_Gender_AllowedValues, string.Join(", ", ValidGenders)));
    }
}
