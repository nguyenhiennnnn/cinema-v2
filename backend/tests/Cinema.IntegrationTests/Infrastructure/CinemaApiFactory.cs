using Cinema.Application.Interfaces;
using Cinema.IntegrationTests.Fakes;
using Cinema.Shared.Constants;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Cinema.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.PostgreSql;

namespace Cinema.IntegrationTests.Infrastructure;

public class CinemaApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithImage("postgres:16-alpine")
        .Build();

    private Respawner _respawner = null!;

    private static readonly string[] ResettableTables =
    [
        "Payments",
        "BookingTickets",
        "BookingCombos",
        "Bookings",
        "ShowtimeSeats",
        "Showtimes",
        "Reviews",
        "RefreshTokens",
        "UserVerificationTokens",
        "CustomerProfiles",
        "Employees",
        "Users",
    ];

    public FakeEmailService EmailService { get; } = new();
    public FakeGoogleAuthService GoogleAuthService { get; } = new();
    public FakePaymentGateway VnPayGateway { get; } = new(PaymentMethodNames.VnPay);
    public FakePaymentGateway MomoGateway { get; } = new(PaymentMethodNames.Momo);
    public FakePaymentGateway StripeGateway { get; } = new(PaymentMethodNames.Stripe);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            _pgContainer.GetConnectionString()
        );
        builder.UseSetting(
            "Jwt:Secret",
            "integration-test-signing-key-do-not-use-in-prod-32"
        );
        builder.UseSetting("RateLimit:GlobalPermitLimit", "100000");
        builder.UseSetting("RateLimit:AuthPermitLimit", "100000");
        builder.UseSetting("RateLimit:BookingHoldPermitLimit", "100000");

        builder.ConfigureAppConfiguration(
            (_, config) =>
            {
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] =
                            _pgContainer.GetConnectionString(),
                        ["RateLimit:GlobalPermitLimit"] = "100000",
                        ["RateLimit:AuthPermitLimit"] = "100000",
                        ["RateLimit:BookingHoldPermitLimit"] = "100000",
                        ["Jwt:Secret"] = "integration-test-signing-key-do-not-use-in-prod-32",
                    }
                );
            }
        );

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    _pgContainer.GetConnectionString(),
                    sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                );
            });

            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService>(EmailService);

            services.RemoveAll<IGoogleAuthService>();
            services.AddSingleton<IGoogleAuthService>(GoogleAuthService);

            services.RemoveAll<IPaymentGateway>();
            services.AddSingleton<IPaymentGateway>(VnPayGateway);
            services.AddSingleton<IPaymentGateway>(MomoGateway);
            services.AddSingleton<IPaymentGateway>(StripeGateway);

            services.AddHangfire(config => config.UseInMemoryStorage());
        });
    }

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();
        _ = CreateClient();

        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        await using var connection = new NpgsqlConnection(_pgContainer.GetConnectionString());
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToInclude = ResettableTables
                    .Select(t => new Respawn.Graph.Table(t))
                    .ToArray(),
            }
        );
    }

    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_pgContainer.GetConnectionString());
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);

        EmailService.Clear();
        VnPayGateway.CreatePaymentRequests.Clear();
        VnPayGateway.RefundRequests.Clear();
        MomoGateway.CreatePaymentRequests.Clear();
        MomoGateway.RefundRequests.Clear();
        StripeGateway.CreatePaymentRequests.Clear();
        StripeGateway.RefundRequests.Clear();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}

[CollectionDefinition(Name)]
public class CinemaApiCollection : ICollectionFixture<CinemaApiFactory>
{
    public const string Name = "Cinema API";
}
