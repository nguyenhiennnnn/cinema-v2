using Cinema.Application.DTOs.Ai;
using FluentValidation;

namespace Cinema.Application.Validators.Ai;

public class ChatMessageDtoValidator : AbstractValidator<ChatMessageDto>
{
    private static readonly string[] ValidRoles = ["user", "model", "assistant", "system"];

    public ChatMessageDtoValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role không được để trống.")
            .Must(r => ValidRoles.Contains(r.ToLowerInvariant()))
            .WithMessage($"Role phải là một trong: {string.Join(", ", ValidRoles)}");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Nội dung tin nhắn không được để trống.")
            .MaximumLength(2000)
            .WithMessage("Nội dung tin nhắn không được vượt quá 2000 ký tự.");
    }
}

public class AiChatRequestValidator : AbstractValidator<AiChatRequest>
{
    public AiChatRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message không được để trống.")
            .MaximumLength(1000)
            .WithMessage("Message không được vượt quá 1000 ký tự.");

        RuleForEach(x => x.History)
            .SetValidator(new ChatMessageDtoValidator());
    }
}
