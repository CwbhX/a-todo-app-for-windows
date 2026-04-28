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
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);

        var task = await service.CreateTaskAsync(new CreateTaskRequest("Draft phase 1"), ct);

        Assert.Equal(TaskItemStatus.Active, task.Status);
        Assert.Single(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(ct));
        Assert.Equal(TaskEventType.Created, await dbContext.TaskEvents.Select(item => item.EventType).SingleAsync(ct));
    }

    [Fact]
    public async Task MarkDone_sets_completion_state_and_records_event()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Finish foundation"), ct);

        clock.Now = clock.Now.AddHours(1);
        await service.MarkDoneAsync(task.Id, ct);

        var saved = await dbContext.TaskItems.SingleAsync(item => item.Id == task.Id, ct);
        Assert.Equal(TaskItemStatus.Done, saved.Status);
        Assert.Equal(clock.Now, saved.CompletedAt);
        Assert.Contains(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(ct), item => item.EventType == TaskEventType.MarkedDone);
    }

    [Fact]
    public async Task Reopen_clears_completed_at_and_records_event()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Review task flow"), ct);

        await service.MarkDoneAsync(task.Id, ct);
        await service.ReopenAsync(task.Id, ct);

        var saved = await dbContext.TaskItems.SingleAsync(item => item.Id == task.Id, ct);
        Assert.Equal(TaskItemStatus.Active, saved.Status);
        Assert.Null(saved.CompletedAt);
        Assert.Contains(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(ct), item => item.EventType == TaskEventType.Reopened);
    }

    [Fact]
    public async Task Archive_excludes_task_from_active_query_and_records_event()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Archive instead of delete"), ct);

        await service.ArchiveAsync(task.Id, ct);

        Assert.DoesNotContain(await service.GetActiveTasksAsync(ct), item => item.Id == task.Id);
        Assert.Contains(await dbContext.TaskEvents.Where(item => item.TaskItemId == task.Id).ToListAsync(ct), item => item.EventType == TaskEventType.Archived);
    }

    [Fact]
    public async Task Active_query_includes_recently_completed_tasks_inside_grace_period()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Done but still visible"), ct);

        await service.MarkDoneAsync(task.Id, ct);

        Assert.Contains(await service.GetActiveTasksAsync(ct), item => item.Id == task.Id);
    }

    [Fact]
    public async Task Active_query_hides_completed_tasks_after_grace_period()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var task = await service.CreateTaskAsync(new CreateTaskRequest("Completed yesterday"), ct);

        await service.MarkDoneAsync(task.Id, ct);
        clock.Now = clock.Now.AddDays(2);

        Assert.DoesNotContain(await service.GetActiveTasksAsync(ct), item => item.Id == task.Id);
    }

    [Fact]
    public async Task Done_query_returns_completed_tasks_only()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var active = await service.CreateTaskAsync(new CreateTaskRequest("Still active"), ct);
        var done = await service.CreateTaskAsync(new CreateTaskRequest("Finished"), ct);
        var archived = await service.CreateTaskAsync(new CreateTaskRequest("Archived after done"), ct);

        await service.MarkDoneAsync(done.Id, ct);
        await service.MarkDoneAsync(archived.Id, ct);
        await service.ArchiveAsync(archived.Id, ct);

        var results = await service.GetDoneTasksAsync(new DoneQuery(), ct);

        Assert.Contains(results, item => item.Id == done.Id);
        Assert.DoesNotContain(results, item => item.Id == active.Id);
        Assert.DoesNotContain(results, item => item.Id == archived.Id);
    }

    [Fact]
    public async Task Done_query_honors_completed_date_range()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var factory = new TestDbFactory();
        await using var dbContext = await factory.CreateAsync(ct);
        var clock = new FixedClock(new DateTimeOffset(2026, 4, 20, 12, 0, 0, TimeSpan.Zero));
        var service = new TaskService(dbContext, clock);
        var oldTask = await service.CreateTaskAsync(new CreateTaskRequest("Old finish"), ct);
        await service.MarkDoneAsync(oldTask.Id, ct);

        clock.Now = new DateTimeOffset(2026, 4, 27, 12, 0, 0, TimeSpan.Zero);
        var recentTask = await service.CreateTaskAsync(new CreateTaskRequest("Recent finish"), ct);
        await service.MarkDoneAsync(recentTask.Id, ct);

        var results = await service.GetDoneTasksAsync(
            new DoneQuery(CompletedFrom: new DateTimeOffset(2026, 4, 26, 0, 0, 0, TimeSpan.Zero)),
            ct);

        Assert.Contains(results, item => item.Id == recentTask.Id);
        Assert.DoesNotContain(results, item => item.Id == oldTask.Id);
    }
}
