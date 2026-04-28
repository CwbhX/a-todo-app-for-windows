using Microsoft.EntityFrameworkCore;

namespace Linework.Data;

public sealed class DbInitializer(LineworkDbContext dbContext)
{
    public Task InitializeAsync(CancellationToken ct = default)
    {
        return dbContext.Database.EnsureCreatedAsync(ct);
    }
}
