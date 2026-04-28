namespace Linework.Services;

public interface ISearchIndexService
{
    Task RebuildAsync(CancellationToken ct = default);
}

public sealed class SearchIndexService : ISearchIndexService
{
    public Task RebuildAsync(CancellationToken ct = default)
    {
        // TODO Phase 6: create and maintain the SQLite FTS5 index.
        return Task.CompletedTask;
    }
}
