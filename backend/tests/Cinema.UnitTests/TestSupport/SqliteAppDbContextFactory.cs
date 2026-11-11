using Cinema.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Cinema.UnitTests.TestSupport;

internal sealed class SqliteTestAppDbContext(DbContextOptions<AppDbContext> options)
    : AppDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (
                    property.GetDefaultValueSql() is { } sql
                    && (
                        sql.Contains("AT TIME ZONE", StringComparison.OrdinalIgnoreCase)
                        || sql.Contains("NOW()", StringComparison.OrdinalIgnoreCase)
                    )
                )
                {
                    property.SetDefaultValueSql(null);
                }

                if (
                    property.GetColumnType() is { } colType
                    && (
                        colType.Contains("timestamp", StringComparison.OrdinalIgnoreCase)
                        || colType.Contains("decimal", StringComparison.OrdinalIgnoreCase)
                    )
                )
                {
                    property.SetColumnType(null);
                }
            }

            foreach (var checkConstraint in entityType.GetCheckConstraints().ToList())
            {
                entityType.RemoveCheckConstraint(checkConstraint.ModelName);
            }
        }
    }
}

public sealed class SqliteAppDbContextHandle : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Db { get; }

    public SqliteAppDbContextHandle()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        Db = new SqliteTestAppDbContext(options);

        Db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
