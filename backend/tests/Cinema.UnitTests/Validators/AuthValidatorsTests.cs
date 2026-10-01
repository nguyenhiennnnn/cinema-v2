using Cinema.Application.DTOs.Auth;
using Cinema.Application.Validators.Auth;
using FluentValidation.TestHelper;

namespace Cinema.UnitTests.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new RegisterRequest(
            "Nguyen Van A",
            "user@example.com",
            "Str0ngPass",
            "0901234567"
        );

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyFullName_FailsValidation()
    {
        var request = new RegisterRequest("", "user@example.com", "Str0ngPass", null);

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    public void InvalidEmail_FailsValidation(string email)
    {
        var request = new RegisterRequest("Nguyen Van A", email, "Str0ngPass", null);

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("short1A")] // < 8 chars
    [InlineData("alllowercase1")] // no uppercase
    [InlineData("ALLUPPERCASE1")] // no lowercase
    [InlineData("NoDigitsHere")] // no digit
    public void WeakPassword_FailsValidation(string password)
    {
        var request = new RegisterRequest("Nguyen Van A", "user@example.com", password, null);

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PhoneProvided_MustMatchVietnamesePhoneFormat()
    {
        var request = new RegisterRequest(
            "Nguyen Van A",
            "user@example.com",
            "Str0ngPass",
            "12345"
        );

        _validator.TestValidate(request).ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData("0901234567")]
    [InlineData("+84901234567")]
    public void PhoneProvided_ValidVietnamesePhone_Passes(string phone)
    {
        var request = new RegisterRequest("Nguyen Van A", "user@example.com", "Str0ngPass", phone);

        _validator.TestValidate(request).ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void PhoneNotProvided_IsOptionalAndPasses()
    {
        var request = new RegisterRequest("Nguyen Van A", "user@example.com", "Str0ngPass", null);

        _validator.TestValidate(request).ShouldNotHaveValidationErrorFor(x => x.Phone);
    }
}

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var result = _validator.TestValidate(new LoginRequest("user@example.com", "anything"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyPassword_FailsValidation_ButDoesNotCheckStrength()
    {
        // Login must only require a non-empty password — NOT run it through the
        // registration-time "strong password" policy (a real 20-year-old account may
        // have a password that no longer satisfies today's stronger policy).
        var result = _validator.TestValidate(new LoginRequest("user@example.com", ""));

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void ShortWeakPassword_StillPassesValidation()
    {
        var result = _validator.TestValidate(new LoginRequest("user@example.com", "a"));

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}

public class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public void EmptyToken_FailsValidation()
    {
        _validator
            .TestValidate(new ResetPasswordRequest("", "Str0ngPass"))
            .ShouldHaveValidationErrorFor(x => x.Token);
    }

    [Fact]
    public void WeakNewPassword_FailsValidation()
    {
        _validator
            .TestValidate(new ResetPasswordRequest("token", "weak"))
            .ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        _validator
            .TestValidate(new ResetPasswordRequest("token", "Str0ngPass"))
            .ShouldNotHaveAnyValidationErrors();
    }
}

public class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordRequestValidator _validator = new();

    [Fact]
    public void EmptyCurrentPassword_FailsValidation()
    {
        _validator
            .TestValidate(new ChangePasswordRequest("", "Str0ngPass"))
            .ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Fact]
    public void WeakNewPassword_FailsValidation()
    {
        _validator
            .TestValidate(new ChangePasswordRequest("current", "weak"))
            .ShouldHaveValidationErrorFor(x => x.NewPassword);
    }
}
