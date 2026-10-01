using FluentValidation;

namespace Cinema.Application.Validators.Auth;

public static class PasswordValidationExtensions
{
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(
        this IRuleBuilder<T, string> ruleBuilder
    ) =>
        ruleBuilder
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
            .Matches("[A-Z]")
            .WithMessage("Mật khẩu phải có ít nhất 1 chữ hoa.")
            .Matches("[a-z]")
            .WithMessage("Mật khẩu phải có ít nhất 1 chữ thường.")
            .Matches("[0-9]")
            .WithMessage("Mật khẩu phải có ít nhất 1 chữ số.");
}
