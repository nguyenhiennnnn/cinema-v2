using Cinema.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Cinema.UnitTests.TestSupport;

public sealed class SqliteAppDbContextHandle : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Db { get; }

    public SqliteAppDbContextHandle()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        Db = new AppDbContext(options);

        Db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
