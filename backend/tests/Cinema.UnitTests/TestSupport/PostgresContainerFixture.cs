using Testcontainers.PostgreSql;

namespace Cinema.UnitTests.TestSupport;

/// <summary>
/// One PostgreSQL container shared for the lifetime of a test class.
/// Each test method gets an isolated database via <see cref="PostgresAppDbContextHandle"/>.
/// </summary>
public sealed class PostgresContainerFixture : IDisposable
{
    private readonly PostgreSqlContainer _container;

    public string MasterConnectionString { get; }

    // The whole app uses "timestamp without time zone" columns with UTC semantics.
    // Npgsql 6+ strict mode rejects comparing DateTime.UtcNow (Kind=Utc) against
    // those columns in LINQ queries. Legacy mode restores the pre-v6 behaviour where
    // all DateTime values map to "timestamp without time zone" regardless of Kind.
    // Must be set before ANY Npgsql query runs; a static ctor runs once per process.
    static PostgresContainerFixture() =>
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    public PostgresContainerFixture()
    {
        // The whole app uses "timestamp without time zone" columns with UTC semantics.
        // Npgsql 6+ strict mode rejects comparing DateTime.UtcNow (Kind=Utc) against
        // those columns in LINQ queries. Legacy mode restores the pre-v6 behaviour where
        // all DateTime values map to "timestamp without time zone" regardless of Kind.
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        _container = new PostgreSqlBuilder("postgres:16-alpine")
            .Build();

        _container.StartAsync().GetAwaiter().GetResult();
        MasterConnectionString = _container.GetConnectionString();
    }

    public void Dispose()
    {
        _container.DisposeAsync().GetAwaiter().GetResult();
    }
}
