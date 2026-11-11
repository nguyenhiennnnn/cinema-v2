using Cinema.Application.DTOs.Auth;

namespace Cinema.Application.Interfaces;

public interface IAuthService
{
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
    Task<MessageResponse> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken);
    Task<MeResponse> GetMeAsync(int userId, CancellationToken cancellationToken);
    Task<SocketTokenResponse> GenerateSocketTokenAsync(
        int userId,
        CancellationToken cancellationToken
    );
}
