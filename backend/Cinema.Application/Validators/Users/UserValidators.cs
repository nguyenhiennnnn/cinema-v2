using System.Text.RegularExpressions;
using Cinema.Application.DTOs.Users;
using Cinema.Domain.Enums;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Users;

public partial class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    private static readonly string[] ValidGenders = Enum.GetNames<Gender>();

    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone)
            .Matches(PhoneRegex())
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage(Messages.Validation_Phone_Invalid)
            .MaximumLength(20);
        RuleFor(x => x.Avatar).MaximumLength(255);
        RuleFor(x => x.Gender)
            .Must(g => ValidGenders.Contains(g))
            .When(x => x.Gender is not null)
            .WithMessage(string.Format(Messages.Validation_Gender_AllowedValues, string.Join(", ", ValidGenders)));
    }

    [GeneratedRegex(@"^(0|\+84)[0-9]{9,10}$")]
    private static partial Regex PhoneRegex();
}
