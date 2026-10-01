namespace Cinema.Application.DTOs.Auth;

public record RegisterRequest(string FullName, string Email, string Password, string? Phone);

public record RegisterResponse(int UserId, string Email, string Message);

public record MessageResponse(string Message);

public record ConfirmEmailRequest(string Token);

public record LoginRequest(string Email, string Password);

public record GoogleLoginRequest(string IdToken);

public record LoginUserDto(int UserId, string FullName, string Email, string Role, string? Avatar, string? MembershipTierName, int? Points, bool IsEmailConfirmed);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    LoginUserDto User
);

public record RefreshRequest(string RefreshToken);

public record RefreshResponse(
    string AccessToken,
    string RefreshToken,
    LoginUserDto User,
    int ExpiresIn
);

public record LogoutRequest(string RefreshToken);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Token, string NewPassword);

public record ResendConfirmationEmailRequest(string Email);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record SocketTokenResponse(string Token, int ExpiresIn);

public record MeResponse(
    int UserId,
    string FullName,
    string Email,
    string? Phone,
    string? Avatar,
    string Role
);
