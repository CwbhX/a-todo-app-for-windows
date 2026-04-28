using Linework.Data;
using Linework.Infrastructure;
using Linework.Services;
using Linework.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Linework.App;

public partial class App
{
    private IHost? _host;

    protected override async void OnStartup(System.Windows.StartupEventArgs e)
    {
        base.OnStartup(e);

        AppPaths.EnsureDirectories();

        _host = Host.CreateDefaultBuilder()
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

        await _host.StartAsync();

        using (var scope = _host.Services.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<DbInitializer>().InitializeAsync();
        }

        _host.Services.GetRequiredService<MainWindow>().Show();
    }

    protected override async void OnExit(System.Windows.ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
