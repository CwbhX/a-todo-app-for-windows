using System.Text.Json;
using Linework.Models;
using Linework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Linework.Data;

public sealed class DbInitializer(LineworkDbContext dbContext, ILogger<DbInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        logger.LogDebug("Ensuring Linework database exists.");
        await dbContext.Database.EnsureCreatedAsync(ct);
        await EnsureDefaultSettingAsync(SettingKeys.ThemeMode, AppThemeMode.System, ct);
        await EnsureDefaultSettingAsync(SettingKeys.DefaultView, AppDefaultView.Today, ct);
        await EnsureDefaultSettingAsync(SettingKeys.DoneGracePeriodDays, 1, ct);
        await dbContext.SaveChangesAsync(ct);
        logger.LogDebug("Default settings ensured.");
    }

    private async Task EnsureDefaultSettingAsync<T>(string key, T value, CancellationToken ct)
    {
        if (await dbContext.AppSettings.AnyAsync(setting => setting.Key == key, ct))
        {
            logger.LogTrace("Setting {SettingKey} already exists.", key);
            return;
        }

        logger.LogDebug("Creating default setting {SettingKey}.", key);
        dbContext.AppSettings.Add(new AppSetting
        {
            Key = key,
            ValueJson = JsonSerializer.Serialize(value, SettingsJson.Options)
        });
    }
}
