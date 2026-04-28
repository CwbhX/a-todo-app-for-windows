namespace Linework.Services;

public interface ISummaryService
{
    Task<string> CreatePlaceholderSummaryAsync(CancellationToken ct = default);
}

public sealed class SummaryService : ISummaryService
{
    public Task<string> CreatePlaceholderSummaryAsync(CancellationToken ct = default)
    {
        return Task.FromResult("Summary generation is planned for a later phase.");
    }
}
