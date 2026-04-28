using Linework.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Linework.Tests;

public sealed class TestDbFactory : IAsyncDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public async Task<LineworkDbContext> CreateAsync()
    {
        await _connection.OpenAsync();
        var options = new DbContextOptionsBuilder<LineworkDbContext>()
            .UseSqlite(_connection)
            .Options;

        var dbContext = new LineworkDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        return dbContext;
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
