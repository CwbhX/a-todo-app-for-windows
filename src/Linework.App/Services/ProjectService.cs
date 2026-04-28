using Linework.Data;
using Linework.Infrastructure;
using Linework.Models;
using Microsoft.EntityFrameworkCore;

namespace Linework.Services;

public interface IProjectService
{
    Task<Project> CreateProjectAsync(string name, CancellationToken ct = default);
    Task<IReadOnlyList<Project>> GetActiveProjectsAsync(CancellationToken ct = default);
}

public sealed class ProjectService(LineworkDbContext dbContext, IClock clock) : IProjectService
{
    public async Task<Project> CreateProjectAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Project name is required.", nameof(name));
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            CreatedAt = clock.Now.ToUniversalTime()
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(ct);
        return project;
    }

    public async Task<IReadOnlyList<Project>> GetActiveProjectsAsync(CancellationToken ct = default)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.ArchivedAt == null)
            .OrderBy(project => project.SortOrder)
            .ThenBy(project => project.Name)
            .ToListAsync(ct);
    }
}
