using Cinema.Application.DTOs.Employees;
using FluentValidation;

namespace Cinema.Application.Validators.Employees;

public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("roleId phải lớn hơn 0.");
    }
}

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("userId phải lớn hơn 0.");

        RuleFor(x => x.PositionId)
            .GreaterThan(0)
            .When(x => x.PositionId is not null)
            .WithMessage("positionId phải lớn hơn 0.");
    }
}

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.PositionId)
            .GreaterThan(0)
            .When(x => x.PositionId is not null)
            .WithMessage("positionId phải lớn hơn 0.");
    }
}
