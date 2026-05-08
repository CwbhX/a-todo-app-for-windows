using Linework.Data;
using Linework.Infrastructure;
using Linework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Linework.Services;

public sealed record CreateTaskRequest(
    string Title,
    string? MarkdownNotes = null,
    bool IsImportant = false,
    Guid? ProjectId = null,
    DateOnly? PlannedForDate = null,
    DateTimeOffset? DueAt = null,
    DateTimeOffset? ReminderAt = null);

public sealed record UpdateTaskDetailsRequest(
    string Title,
    string? MarkdownNotes = null);

public sealed record DoneQuery(DateTimeOffset? CompletedFrom = null, DateTimeOffset? CompletedTo = null);

public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task<TaskItem?> GetTaskAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetTodayTasksAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetActiveTasksAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetDoneTasksAsync(DoneQuery query, CancellationToken ct = default);
    Task UpdateTaskDetailsAsync(Guid taskId, UpdateTaskDetailsRequest request, CancellationToken ct = default);
    Task PlanForTodayAsync(Guid taskId, CancellationToken ct = default);
    Task MarkDoneAsync(Guid taskId, CancellationToken ct = default);
    Task ReopenAsync(Guid taskId, CancellationToken ct = default);
    Task ArchiveAsync(Guid taskId, CancellationToken ct = default);
}

public sealed class TaskService(
    LineworkDbContext dbContext,
    IClock clock,
    ISettingsService settingsService,
    ILogger<TaskService> logger) : ITaskService
{
    private const int DefaultDoneGracePeriodDays = 1;

    public async Task<TaskItem> CreateTaskAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.", nameof(request));
        }

        var now = clock.Now.ToUniversalTime();
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            MarkdownNotes = request.MarkdownNotes,
            IsImportant = request.IsImportant,
            ProjectId = request.ProjectId,
            PlannedForDate = request.PlannedForDate,
            DueAt = request.DueAt?.ToUniversalTime(),
            ReminderAt = request.ReminderAt?.ToUniversalTime(),
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.TaskItems.Add(task);
        AddEvent(task.Id, TaskEventType.Created, now);
        await dbContext.SaveChangesAsync(ct);
        logger.LogDebug(
            "Created task {TaskId} with title '{TaskTitle}' planned for {PlannedForDate}.",
            task.Id,
            task.Title,
            task.PlannedForDate);
        return task;
    }

    public Task<TaskItem?> GetTaskAsync(Guid id, CancellationToken ct = default)
    {
        return dbContext.TaskItems
            .AsNoTracking()
            .Include(task => task.Project)
            .FirstOrDefaultAsync(task => task.Id == id, ct);
    }

    public async Task<IReadOnlyList<TaskItem>> GetTodayTasksAsync(CancellationToken ct = default)
    {
        var today = clock.Today;
        var tasks = await dbContext.TaskItems
            .AsNoTracking()
            .Include(task => task.Project)
            .Where(task => task.Status != TaskItemStatus.Archived)
            .ToListAsync(ct);

        return tasks
            .Where(task =>
                task.PlannedForDate == today ||
                IsLocalDate(task.DueAt, today) ||
                IsLocalDate(task.CompletedAt, today))
            .OrderByDescending(task => task.IsImportant)
            .ThenBy(task => task.Status)
            .ThenBy(task => task.SortOrder)
            .ThenByDescending(task => task.UpdatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<TaskItem>> GetActiveTasksAsync(CancellationToken ct = default)
    {
        var doneGracePeriod = await GetDoneGracePeriodAsync(ct);
        var cutoff = clock.Now.ToUniversalTime().Subtract(doneGracePeriod);

        return await dbContext.TaskItems
            .AsNoTracking()
            .Include(task => task.Project)
            .Where(task =>
                task.Status == TaskItemStatus.Active ||
                (task.Status == TaskItemStatus.Done && task.CompletedAt >= cutoff))
            .OrderByDescending(task => task.IsImportant)
            .ThenBy(task => task.Status)
            .ThenBy(task => task.SortOrder)
            .ThenByDescending(task => task.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TaskItem>> GetDoneTasksAsync(DoneQuery query, CancellationToken ct = default)
    {
        IQueryable<TaskItem> tasks = dbContext.TaskItems
            .AsNoTracking()
            .Include(task => task.Project)
            .Where(task => task.Status == TaskItemStatus.Done);

        if (query.CompletedFrom is not null)
        {
            var from = query.CompletedFrom.Value.ToUniversalTime();
            tasks = tasks.Where(task => task.CompletedAt >= from);
        }

        if (query.CompletedTo is not null)
        {
            var to = query.CompletedTo.Value.ToUniversalTime();
            tasks = tasks.Where(task => task.CompletedAt <= to);
        }

        return await tasks
            .OrderByDescending(task => task.CompletedAt)
            .ThenBy(task => task.Title)
            .ToListAsync(ct);
    }

    public async Task UpdateTaskDetailsAsync(Guid taskId, UpdateTaskDetailsRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Task title is required.", nameof(request));
        }

        var task = await FindTaskForUpdateAsync(taskId, ct);
        var title = request.Title.Trim();
        var markdownNotes = string.IsNullOrWhiteSpace(request.MarkdownNotes)
            ? null
            : request.MarkdownNotes.Trim();

        if (task.Title == title && task.MarkdownNotes == markdownNotes)
        {
            return;
        }

        var now = clock.Now.ToUniversalTime();
        task.Title = title;
        task.MarkdownNotes = markdownNotes;
        task.UpdatedAt = now;
        AddEvent(task.Id, TaskEventType.Edited, now);

        await dbContext.SaveChangesAsync(ct);
        logger.LogDebug("Updated task details for {TaskId} with title '{TaskTitle}'.", task.Id, task.Title);
    }

    public async Task PlanForTodayAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await FindTaskForUpdateAsync(taskId, ct);
        var now = clock.Now.ToUniversalTime();

        task.PlannedForDate = clock.Today;
        task.UpdatedAt = now;
        AddEvent(task.Id, TaskEventType.PlannedForDateChanged, now);

        await dbContext.SaveChangesAsync(ct);
        logger.LogDebug("Planned task {TaskId} for today ({PlannedForDate}).", task.Id, task.PlannedForDate);
    }

    public async Task MarkDoneAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await FindTaskForUpdateAsync(taskId, ct);
        var now = clock.Now.ToUniversalTime();

        task.Status = TaskItemStatus.Done;
        task.CompletedAt = now;
        task.ArchivedAt = null;
        task.UpdatedAt = now;
        AddEvent(task.Id, TaskEventType.MarkedDone, now);

        await dbContext.SaveChangesAsync(ct);
        logger.LogDebug("Marked task {TaskId} done at {CompletedAt}.", task.Id, task.CompletedAt);
    }

    public async Task ReopenAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await FindTaskForUpdateAsync(taskId, ct);
        var now = clock.Now.ToUniversalTime();

        task.Status = TaskItemStatus.Active;
        task.CompletedAt = null;
        task.ArchivedAt = null;
        task.UpdatedAt = now;
        AddEvent(task.Id, TaskEventType.Reopened, now);

        await dbContext.SaveChangesAsync(ct);
        logger.LogDebug("Reopened task {TaskId}.", task.Id);
    }

    public async Task ArchiveAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await FindTaskForUpdateAsync(taskId, ct);
        var now = clock.Now.ToUniversalTime();

        task.Status = TaskItemStatus.Archived;
        task.ArchivedAt = now;
        task.UpdatedAt = now;
        AddEvent(task.Id, TaskEventType.Archived, now);

        await dbContext.SaveChangesAsync(ct);
        logger.LogDebug("Archived task {TaskId} at {ArchivedAt}.", task.Id, task.ArchivedAt);
    }

    private async Task<TaskItem> FindTaskForUpdateAsync(Guid taskId, CancellationToken ct)
    {
        return await dbContext.TaskItems.FirstOrDefaultAsync(task => task.Id == taskId, ct)
            ?? throw new InvalidOperationException($"Task '{taskId}' was not found.");
    }

    private void AddEvent(Guid taskId, TaskEventType eventType, DateTimeOffset occurredAt)
    {
        dbContext.TaskEvents.Add(new TaskEvent
        {
            Id = Guid.NewGuid(),
            TaskItemId = taskId,
            EventType = eventType,
            OccurredAt = occurredAt
        });
    }

    private static bool IsLocalDate(DateTimeOffset? value, DateOnly date)
    {
        return value.HasValue && DateOnly.FromDateTime(value.Value.LocalDateTime) == date;
    }

    private async Task<TimeSpan> GetDoneGracePeriodAsync(CancellationToken ct)
    {
        var configuredDays = await settingsService.GetAsync<int?>(SettingKeys.DoneGracePeriodDays, ct);
        return TimeSpan.FromDays(Math.Max(0, configuredDays ?? DefaultDoneGracePeriodDays));
    }
}
