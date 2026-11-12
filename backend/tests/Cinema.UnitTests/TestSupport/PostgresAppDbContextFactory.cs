using Cinema.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Cinema.UnitTests.TestSupport;

/// <summary>
/// Creates a unique PostgreSQL database inside a shared container for one test method.
/// Drops the database on dispose, giving full per-test isolation without spawning
/// a new container per test.
/// </summary>
public sealed class PostgresAppDbContextHandle : IDisposable
{
    private readonly string _masterConnectionString;
    private readonly string _dbName;

    public AppDbContext Db { get; }

    public PostgresAppDbContextHandle(string masterConnectionString)
    {
        _masterConnectionString = masterConnectionString;
        _dbName = "ut_" + Guid.NewGuid().ToString("N");

        // Create the isolated database
        using var adminConn = new NpgsqlConnection(masterConnectionString);
        adminConn.Open();
        using (var cmd = adminConn.CreateCommand())
        {
            cmd.CommandText = $"CREATE DATABASE \"{_dbName}\"";
            cmd.ExecuteNonQuery();
        }

        // Connect to the new database and build schema
        var connStr = BuildConnectionString(masterConnectionString, _dbName);
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connStr)
            .Options;

        Db = new AppDbContext(options);
        Db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Db.Dispose();

        // Drop the isolated database; use a fresh connection to the master DB
        using var adminConn = new NpgsqlConnection(_masterConnectionString);
        adminConn.Open();
        using var cmd = adminConn.CreateCommand();
        // Terminate existing connections then drop
        cmd.CommandText = $"""
            SELECT pg_terminate_backend(pid)
            FROM pg_stat_activity
            WHERE datname = '{_dbName}' AND pid <> pg_backend_pid();
            DROP DATABASE IF EXISTS "{_dbName}";
            """;
        cmd.ExecuteNonQuery();
    }

    private static string BuildConnectionString(string master, string dbName)
    {
        var builder = new NpgsqlConnectionStringBuilder(master) { Database = dbName };
        return builder.ConnectionString;
    }
}
