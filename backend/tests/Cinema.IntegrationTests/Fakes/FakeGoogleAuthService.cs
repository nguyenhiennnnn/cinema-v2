using Cinema.Application.Interfaces;

namespace Cinema.IntegrationTests.Fakes;

public class FakeGoogleAuthService : IGoogleAuthService
{
    public GoogleUserInfo? NextResult { get; set; }

    public Task<GoogleUserInfo> VerifyIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult(
            NextResult
                ?? throw new InvalidOperationException(
                    "FakeGoogleAuthService.NextResult was not configured for this test."
                )
        );
}
