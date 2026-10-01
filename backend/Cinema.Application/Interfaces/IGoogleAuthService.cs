namespace Cinema.Application.Interfaces;

public record GoogleUserInfo(
    string GoogleId,
    string Email,
    bool EmailVerified,
    string? FullName,
    string? Picture
);

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken);
}
