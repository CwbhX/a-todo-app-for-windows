using System.Text.Json;
using Linework.Data;
using Linework.Models;
using Microsoft.EntityFrameworkCore;

namespace Linework.Services;

public interface ISettingsService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, CancellationToken ct = default);
}

public sealed class SettingsService(LineworkDbContext dbContext) : ISettingsService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var setting = await dbContext.AppSettings.AsNoTracking().FirstOrDefaultAsync(item => item.Key == key, ct);
        return setting is null ? default : JsonSerializer.Deserialize<T>(setting.ValueJson);
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken ct = default)
    {
        var valueJson = JsonSerializer.Serialize(value);
        var setting = await dbContext.AppSettings.FirstOrDefaultAsync(item => item.Key == key, ct);

        if (setting is null)
        {
            dbContext.AppSettings.Add(new AppSetting { Key = key, ValueJson = valueJson });
        }
        else
        {
            setting.ValueJson = valueJson;
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
