using Cinema.Application.Interfaces;
using Cinema.Application.Options;
using Cinema.Application.Services.Auth;
using Cinema.Application.Services.Notifications;
using Cinema.Domain.Entities;
using Cinema.Infrastructure.Persistence;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Cinema.UnitTests.TestSupport;

public abstract class AuthServiceTestBase : IDisposable
{
    private readonly SqliteAppDbContextHandle _handle = new();

    protected AppDbContext Db => _handle.Db;
    protected Mock<ITokenService> TokenServiceMock { get; } = new();
    protected Mock<IGoogleAuthService> GoogleAuthServiceMock { get; } = new();
    protected Mock<IPasswordHasher<User>> PasswordHasherMock { get; } = new();
    protected Mock<IBackgroundJobClient> BackgroundJobClientMock { get; } = new();
    protected int RefreshTokenExpirationDays { get; set; } = 30;

    static AuthServiceTestBase()
    {
        Mapster.TypeAdapterConfig.GlobalSettings.Scan(
            typeof(Cinema.Application.MappingConfig.MapsterConfig).Assembly
        );
    }

    protected AuthServiceTestBase()
    {
        TokenServiceMock
            .Setup(t => t.GenerateOpaqueToken())
            .Returns(() => Guid.NewGuid().ToString("N"));
        TokenServiceMock
            .Setup(t => t.HashOpaqueToken(It.IsAny<string>()))
            .Returns((string s) => $"hash:{s}");
        TokenServiceMock
            .Setup(t => t.GenerateAccessToken(It.IsAny<User>()))
            .Returns(new AccessTokenResult("fake-jwt", 3600));
        PasswordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
            .Returns((User _, string p) => $"hashed:{p}");
    }

    protected AuthService CreateAuthService()
    {
        Db.ChangeTracker.Clear();

        return new(
            Db,
            TokenServiceMock.Object,
            GoogleAuthServiceMock.Object,
            PasswordHasherMock.Object,
            Options.Create(
                new JwtOptions { RefreshTokenExpirationDays = RefreshTokenExpirationDays }
            ),
            NullLogger<AuthService>.Instance
        );
    }

    protected RegistrationService CreateRegistrationService()
    {
        Db.ChangeTracker.Clear();

        return new(
            Db,
            TokenServiceMock.Object,
            PasswordHasherMock.Object,
            Options.Create(new EmailOptions()),
            CreateVerificationTokenSender(),
            NullLogger<RegistrationService>.Instance
        );
    }

    protected PasswordService CreatePasswordService()
    {
        Db.ChangeTracker.Clear();

        return new(
            Db,
            TokenServiceMock.Object,
            PasswordHasherMock.Object,
            Options.Create(new EmailOptions()),
            CreateVerificationTokenSender(),
            NullLogger<PasswordService>.Instance
        );
    }

    private VerificationTokenSender CreateVerificationTokenSender() =>
        new(
            Db,
            TokenServiceMock.Object,
            Options.Create(new AppUrlOptions()),
            new NotificationService(BackgroundJobClientMock.Object, Options.Create(new BrandOptions())),
            NullLogger<VerificationTokenSender>.Instance
        );

    public void Dispose() => _handle.Dispose();
}
