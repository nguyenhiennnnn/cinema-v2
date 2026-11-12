using Cinema.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Cinema.UnitTests.TestSupport;

public sealed class PostgresAppDbContextHandle : IDisposable
{
    private readonly PostgreSqlContainer _container;

    public AppDbContext Db { get; }

    private PostgresAppDbContextHandle(PostgreSqlContainer container, AppDbContext db)
    {
        _container = container;
        Db = db;
    }

    public static PostgresAppDbContextHandle Create()
    {
        var container = new PostgreSqlBuilder("postgres:16-alpine")
            .Build();

        container.StartAsync().GetAwaiter().GetResult();

        var connStr = container.GetConnectionString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connStr)
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated();

        return new PostgresAppDbContextHandle(container, db);
    }

    public void Dispose()
    {
        Db.Dispose();
        _container.DisposeAsync().GetAwaiter().GetResult();
    }
}
