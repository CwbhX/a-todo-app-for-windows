using Linework.Data;
using Linework.Models;
using Linework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Linework.Tests;

public sealed class DbInitializerTests
{
    [Fact]
    public async Task Initialize_creates_database_and_seeds_default_settings()
    {
        var ct = TestContext.Current.CancellationToken;
        var directory = Path.Combine(Path.GetTempPath(), "Linework.Tests", Guid.NewGuid().ToString("N"));
        var dbPath = Path.Combine(directory, "linework.db");
        Directory.CreateDirectory(directory);

        try
        {
            var options = new DbContextOptionsBuilder<LineworkDbContext>()
                .UseSqlite($"Data Source={dbPath};Pooling=False")
                .Options;

            await using (var dbContext = new LineworkDbContext(options))
            {
                var initializer = new DbInitializer(dbContext, NullLogger<DbInitializer>.Instance);
                await initializer.InitializeAsync(ct);

                var settings = new SettingsService(dbContext);

                Assert.True(File.Exists(dbPath));
                Assert.Equal(AppThemeMode.System, await settings.GetAsync<AppThemeMode>(SettingKeys.ThemeMode, ct));
                Assert.Equal(AppDefaultView.Today, await settings.GetAsync<AppDefaultView>(SettingKeys.DefaultView, ct));
                Assert.Equal(1, await settings.GetAsync<int>(SettingKeys.DoneGracePeriodDays, ct));

                await initializer.InitializeAsync(ct);

                Assert.Equal(3, await dbContext.AppSettings.CountAsync(ct));
            }
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
