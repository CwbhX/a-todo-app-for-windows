namespace Linework.Services;

public interface IExportService
{
    Task ExportCompletedTasksAsync(CancellationToken ct = default);
}

public sealed class ExportService : IExportService
{
    public Task ExportCompletedTasksAsync(CancellationToken ct = default)
    {
        // TODO Phase 7: export completed tasks to Markdown.
        return Task.CompletedTask;
    }
}
