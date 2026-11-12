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

    public PostgresContainerFixture()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .Build();

        _container.StartAsync().GetAwaiter().GetResult();
        MasterConnectionString = _container.GetConnectionString();
    }

    public void Dispose()
    {
        _container.DisposeAsync().GetAwaiter().GetResult();
    }
}
