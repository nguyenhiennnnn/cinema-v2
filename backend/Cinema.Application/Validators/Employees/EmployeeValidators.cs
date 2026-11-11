using Cinema.Application.DTOs.Employees;
using Cinema.Application.Utils;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Employees;

public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage(Messages.Validation_RoleId_Positive);
    }
}

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage(Messages.Validation_UserId_Positive);

        RuleFor(x => x.PositionId)
            .GreaterThan(0)
            .When(x => x.PositionId is not null)
            .WithMessage(Messages.Validation_PositionId_Positive);
    }
}

public class InviteEmployeeRequestValidator : AbstractValidator<InviteEmployeeRequest>
{
    public InviteEmployeeRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(Messages.Validation_FullName_Required)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(Messages.Validation_Email_Required)
            .EmailAddress()
            .WithMessage(Messages.Validation_Email_Invalid)
            .MaximumLength(150)
            .WithMessage(Messages.Validation_Email_MaxLength150);

        RuleFor(x => x.Phone)
            .Matches(PhoneRules.Pattern)
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage(Messages.Validation_Phone_Invalid)
            .MaximumLength(20)
            .WithMessage(Messages.Validation_Phone_MaxLength20);

        RuleFor(x => x.PositionId)
            .GreaterThan(0)
            .When(x => x.PositionId is not null)
            .WithMessage(Messages.Validation_PositionId_Positive);
    }
}

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.PositionId)
            .GreaterThan(0)
            .When(x => x.PositionId is not null)
            .WithMessage(Messages.Validation_PositionId_Positive);
    }
}
