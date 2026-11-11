using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;

namespace Cinema.Application.Interfaces;

public interface IRegistrationService
{
    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken
    );
    Task<MessageResponse> ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken
    );
    Task<MessageResponse> ResendConfirmationEmailAsync(
        ResendConfirmationEmailRequest request,
        CancellationToken cancellationToken
    );
    Task SendStaffInviteAsync(User user, CancellationToken cancellationToken);
}
