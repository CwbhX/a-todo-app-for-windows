using Linework.Data;
using Linework.Models;
using Microsoft.EntityFrameworkCore;

namespace Linework.Services;

public interface ISearchService
{
    Task<IReadOnlyList<TaskItem>> SearchTasksAsync(string query, bool includeArchived = false, CancellationToken ct = default);
}

public sealed class SearchService(LineworkDbContext dbContext) : ISearchService
{
    public async Task<IReadOnlyList<TaskItem>> SearchTasksAsync(string query, bool includeArchived = false, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var trimmed = query.Trim();

        return await dbContext.TaskItems
            .AsNoTracking()
            .Include(task => task.Project)
            .Where(task => includeArchived || task.Status != TaskItemStatus.Archived)
            .Where(task =>
                task.Title.Contains(trimmed) ||
                (task.MarkdownNotes != null && task.MarkdownNotes.Contains(trimmed)) ||
                (task.Project != null && task.Project.Name.Contains(trimmed)))
            .OrderByDescending(task => task.UpdatedAt)
            .ToListAsync(ct);
    }
}
