using Cinema.Application.DTOs.Import;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Import;

public class ImportApplyRequestValidator : AbstractValidator<ImportApplyRequest>
{
    public ImportApplyRequestValidator()
    {
        RuleFor(x => x.PreviewId)
            .NotEmpty()
            .WithMessage(Messages.Validation_PreviewId_Required)
            .MaximumLength(64)
            .WithMessage(Messages.Validation_PreviewId_MaxLength64);
    }
}
