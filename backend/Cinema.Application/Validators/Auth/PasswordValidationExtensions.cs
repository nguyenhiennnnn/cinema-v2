using FluentValidation;
using Cinema.Shared.Resources;

namespace Cinema.Application.Validators.Auth;

public static class PasswordValidationExtensions
{
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder
    ) =>
        ruleBuilder
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage(Messages.Validation_PasswordMinLength8)
            .Matches("[A-Z]")
            .WithMessage(Messages.Validation_PasswordUppercase)
            .Matches("[a-z]")
            .WithMessage(Messages.Validation_PasswordLowercase)
            .Matches("[0-9]")
            .WithMessage(Messages.Validation_PasswordDigit);
}
