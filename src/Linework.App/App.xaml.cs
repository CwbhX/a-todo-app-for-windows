using Linework.Data;
using Linework.Infrastructure;
using Linework.Services;
using Linework.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Linework.App;

public partial class App
{
    private IHost? _host;

    protected override async void OnStartup(System.Windows.StartupEventArgs e)
    {
        base.OnStartup(e);

        AppPaths.EnsureDirectories();
        var debugLogOptions = DebugLogOptions.Parse(e.Args);

        _host = Host.CreateDefaultBuilder(e.Args)
            .ConfigureLogging(logging =>
            {
                if (debugLogOptions.IsEnabled)
                {
                    logging.AddProvider(new DebugFileLoggerProvider(
                        debugLogOptions.FilePath,
                        LogLevel.Debug,
                        ["Microsoft.EntityFrameworkCore.Database.Command"]));
                }
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton<IClock, SystemClock>();
                services.AddDbContext<LineworkDbContext>(options =>
                    options.UseSqlite($"Data Source={AppPaths.DatabasePath}"));
                services.AddScoped<DbInitializer>();
                services.AddScoped<ITaskService, TaskService>();
                services.AddScoped<IProjectService, ProjectService>();
                services.AddScoped<ISearchService, SearchService>();
                services.AddScoped<ISearchIndexService, SearchIndexService>();
                services.AddSingleton<IMarkdownService, MarkdownService>();
                services.AddSingleton<ISummaryService, SummaryService>();
                services.AddSingleton<IExportService, ExportService>();
                services.AddSingleton<IReminderService, ReminderService>();
                services.AddSingleton<IUrlOpener, UrlOpener>();
                services.AddScoped<ISettingsService, SettingsService>();
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Linework startup began.");
        if (debugLogOptions.IsEnabled)
        {
            logger.LogInformation("Debug file logging enabled at {FilePath}.", debugLogOptions.FilePath);
        }

        await _host.StartAsync();

        using (var scope = _host.Services.CreateScope())
        {
            try
            {
                logger.LogDebug("Database initialization began.");
                await scope.ServiceProvider.GetRequiredService<DbInitializer>().InitializeAsync();
                logger.LogDebug("Database initialization completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed.");
                throw;
            }
        }

        _host.Services.GetRequiredService<MainWindow>().Show();
        logger.LogInformation("Main window shown.");
    }

    protected override async void OnExit(System.Windows.ExitEventArgs e)
    {
        if (_host is not null)
        {
            _host.Services.GetRequiredService<ILogger<App>>().LogInformation("Linework shutdown began.");
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
