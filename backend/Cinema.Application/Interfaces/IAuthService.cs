using Cinema.Application.DTOs.Auth;

namespace Cinema.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult<LoginResponse>> LoginAsync(
        LoginRequest request,
        bool adminOnly,
        CancellationToken cancellationToken
    );
    Task<AuthResult<LoginResponse>> GoogleLoginAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken
    );
    Task<AuthResult<RefreshResponse>> RefreshAsync(
        string? refreshToken,
        CancellationToken cancellationToken
    );
    Task<MessageResponse> LogoutAsync(string? refreshToken, CancellationToken cancellationToken);
    Task<MeResponse> GetMeAsync(int userId, CancellationToken cancellationToken);
    Task<SocketTokenResponse> GenerateSocketTokenAsync(
        int userId,
        CancellationToken cancellationToken
    );
}
