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
        // Clear the pool for the test-database connection so PostgreSQL lets us drop it
        NpgsqlConnection.ClearAllPools();

        // Drop the isolated database; use a non-pooled connection to the master DB
        // so it is never inside a Npgsql pipeline (DROP DATABASE is rejected in pipelines).
        var noPoolCs = new NpgsqlConnectionStringBuilder(_masterConnectionString)
        {
            Pooling = false,
        }.ConnectionString;

        using var adminConn = new NpgsqlConnection(noPoolCs);
        adminConn.Open();

        // Terminate lingering connections first (separate command — avoids pipeline batching)
        using (var killCmd = adminConn.CreateCommand())
        {
            killCmd.CommandText = $"""
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = '{_dbName}' AND pid <> pg_backend_pid()
                """;
            killCmd.ExecuteNonQuery();
        }

        // Now drop (separate command on the same plain connection — no pipeline)
        using (var dropCmd = adminConn.CreateCommand())
        {
            dropCmd.CommandText = $"DROP DATABASE IF EXISTS \"{_dbName}\"";
            dropCmd.ExecuteNonQuery();
        }
    }

    private static string BuildConnectionString(string master, string dbName)
    {
        var builder = new NpgsqlConnectionStringBuilder(master) { Database = dbName };
        return builder.ConnectionString;
    }
}
