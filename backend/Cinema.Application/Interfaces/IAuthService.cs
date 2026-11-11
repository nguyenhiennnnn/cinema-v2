using Cinema.Application.DTOs.Auth;
using Cinema.Domain.Entities;

namespace Cinema.Application.Interfaces;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken
    );
    Task<MessageResponse> ConfirmEmailAsync(
        ConfirmEmailRequest request,
        CancellationToken cancellationToken
    );
    Task<LoginResponse> LoginAsync(
        LoginRequest request,
        bool adminOnly,
        CancellationToken cancellationToken
    );
    Task<LoginResponse> GoogleLoginAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken
    );
    Task<RefreshResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken);
    Task<MeResponse> GetMeAsync(int userId, CancellationToken cancellationToken);

    Task<MessageResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken);
    Task<MessageResponse> ForgotPasswordAsync(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken
    );
    Task<MessageResponse> ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken
    );
    Task<MessageResponse> ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken
    );

    Task<MessageResponse> ResendConfirmationEmailAsync(
        ResendConfirmationEmailRequest request,
        CancellationToken cancellationToken
    );

    Task SendStaffInviteAsync(User user, CancellationToken cancellationToken);

    Task<SocketTokenResponse> GenerateSocketTokenAsync(
        int userId,
        CancellationToken cancellationToken
    );
}
