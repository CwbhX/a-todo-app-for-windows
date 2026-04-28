using Linework.Models;
using Linework.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Linework.Tests;

public sealed class ProjectServiceTests
{
    [Fact]
    public async Task CreateProject_trims_name_and_persists_created_at()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 8, 30, 0, TimeSpan.FromHours(-7)));
        var service = new ProjectService(dbContext, clock);

        var project = await service.CreateProjectAsync("  Linework  ", ct);
        var saved = await dbContext.Projects.SingleAsync(item => item.Id == project.Id, ct);

        Assert.Equal("Linework", saved.Name);
        Assert.Equal(clock.Now.ToUniversalTime(), saved.CreatedAt);
    }

    [Fact]
    public async Task GetActiveProjects_lists_unarchived_projects_by_sort_order_then_name()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var now = new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero);
        dbContext.Projects.AddRange(
            new Project { Id = Guid.NewGuid(), Name = "Bravo", SortOrder = 1, CreatedAt = now },
            new Project { Id = Guid.NewGuid(), Name = "Alpha", SortOrder = 1, CreatedAt = now },
            new Project { Id = Guid.NewGuid(), Name = "Today", SortOrder = 0, CreatedAt = now },
            new Project { Id = Guid.NewGuid(), Name = "Archived", SortOrder = -1, CreatedAt = now, ArchivedAt = now });
        await dbContext.SaveChangesAsync(ct);
        var service = new ProjectService(dbContext, new FixedClock(now));

        var projects = await service.GetActiveProjectsAsync(ct);

        Assert.Collection(
            projects,
            project => Assert.Equal("Today", project.Name),
            project => Assert.Equal("Alpha", project.Name),
            project => Assert.Equal("Bravo", project.Name));
    }
}
