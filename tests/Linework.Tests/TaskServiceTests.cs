using Linework.Models;
using Linework.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Linework.Tests;

public sealed class TaskServiceTests
{
    [Fact]
    public async Task CreateTask_records_created_event()
    {
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync();
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);

        var task = await service.CreateTaskAsync(new CreateTaskRequest("Draft phase 1"));

        Assert.Equal(TaskItemStatus.Active, task.Status);
        Assert.Single(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync());
        Assert.Equal(TaskEventType.Created, await dbContext.TaskEvents.Select(item => item.EventType).SingleAsync());
    }

    [Fact]
    public async Task MarkDone_sets_completion_state_and_records_event()
    {
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync();
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Finish foundation"));

        clock.Now = clock.Now.AddHours(1);
        await service.MarkDoneAsync(task.Id);

        var saved = await dbContext.TaskItems.SingleAsync(item => item.Id == task.Id);
        Assert.Equal(TaskItemStatus.Done, saved.Status);
        Assert.Equal(clock.Now, saved.CompletedAt);
        Assert.Contains(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(), item => item.EventType == TaskEventType.MarkedDone);
    }

    [Fact]
    public async Task Reopen_clears_completed_at_and_records_event()
    {
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync();
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Review task flow"));

        await service.MarkDoneAsync(task.Id);
        await service.ReopenAsync(task.Id);

        var saved = await dbContext.TaskItems.SingleAsync(item => item.Id == task.Id);
        Assert.Equal(TaskItemStatus.Active, saved.Status);
        Assert.Null(saved.CompletedAt);
        Assert.Contains(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(), item => item.EventType == TaskEventType.Reopened);
    }

    [Fact]
    public async Task Archive_excludes_task_from_active_query_and_records_event()
    {
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync();
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Archive instead of delete"));

        await service.ArchiveAsync(task.Id);

        Assert.DoesNotContain(await service.GetActiveTasksAsync(), item => item.Id == task.Id);
        Assert.Contains(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(), item => item.EventType == TaskEventType.Archived);
    }

    [Fact]
    public async Task Active_query_includes_recently_completed_tasks_inside_grace_period()
    {
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync();
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Done but still visible"));

        await service.MarkDoneAsync(task.Id);

        Assert.Contains(await service.GetActiveTasksAsync(), item => item.Id == task.Id);
    }

    [Fact]
    public async Task Active_query_hides_completed_tasks_after_grace_period()
    {
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync();
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Completed yesterday"));

        await service.MarkDoneAsync(task.Id);
        clock.Now = clock.Now.AddDays(2);

        Assert.DoesNotContain(await service.GetActiveTasksAsync(), item => item.Id == task.Id);
    }
}
