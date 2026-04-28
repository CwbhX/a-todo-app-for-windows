using System.Text.Json;
using Linework.Models;
using Linework.Services;
using Microsoft.EntityFrameworkCore;

namespace Linework.Data;

public sealed class DbInitializer(LineworkDbContext dbContext)
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await dbContext.Database.EnsureCreatedAsync(ct);
        await EnsureDefaultSettingAsync(SettingKeys.ThemeMode, AppThemeMode.System, ct);
        await EnsureDefaultSettingAsync(SettingKeys.DefaultView, AppDefaultView.Today, ct);
        await EnsureDefaultSettingAsync(SettingKeys.DoneGracePeriodDays, 1, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    private async Task EnsureDefaultSettingAsync<T>(string key, T value, CancellationToken ct)
    {
        if (await dbContext.AppSettings.AnyAsync(setting => setting.Key == key, ct))
        {
            return;
        }

        dbContext.AppSettings.Add(new AppSetting
        {
            Key = key,
            ValueJson = JsonSerializer.Serialize(value, SettingsJson.Options)
        });
    }
}
