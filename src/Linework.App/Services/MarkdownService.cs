using Markdig;

namespace Linework.Services;

public interface IMarkdownService
{
    string ToHtmlPreview(string markdown);
}

public sealed class MarkdownService : IMarkdownService
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseTaskLists()
        .Build();

    public string ToHtmlPreview(string markdown)
    {
        return Markdown.ToHtml(markdown ?? string.Empty, _pipeline);
    }
}
