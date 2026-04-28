namespace Linework.Services.Ai;

public interface IAiSummaryProvider
{
    Task<string> GenerateSummaryAsync(object summaryInput, CancellationToken ct = default);
}
