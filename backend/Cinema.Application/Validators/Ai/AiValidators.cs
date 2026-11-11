using Cinema.Application.DTOs.Ai;
using Cinema.Shared.Resources;
using FluentValidation;

namespace Cinema.Application.Validators.Ai;

public class ChatMessageDtoValidator : AbstractValidator<ChatMessageDto>
{
    private static readonly string[] ValidRoles = ["user", "model", "assistant", "system"];

    public ChatMessageDtoValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage(Messages.Validation_Role_Required)
            .Must(r => ValidRoles.Contains(r.ToLowerInvariant()))
            .WithMessage(
                string.Format(Messages.Validation_Role_AllowedValues, string.Join(", ", ValidRoles))
            );

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(Messages.Validation_MessageContent_Required)
            .MaximumLength(2000)
            .WithMessage(Messages.Validation_MessageContent_MaxLength2000);
    }
}

public class AiChatRequestValidator : AbstractValidator<AiChatRequest>
{
    public AiChatRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage(Messages.Validation_Message_Required)
            .MaximumLength(1000)
            .WithMessage(Messages.Validation_Message_MaxLength1000);

        RuleForEach(x => x.History).SetValidator(new ChatMessageDtoValidator());
    }
}
