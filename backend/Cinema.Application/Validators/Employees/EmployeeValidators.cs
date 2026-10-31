using Cinema.Application.DTOs.Employees;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Employees;

public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_RoleId_Positive);
    }
}

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage(Messages.Validation_UserId_Positive);

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
