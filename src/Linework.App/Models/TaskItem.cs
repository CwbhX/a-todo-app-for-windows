namespace Linework.Models;

public sealed class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? MarkdownNotes { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Active;
    public bool IsImportant { get; set; }
    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }
    public DateOnly? PlannedForDate { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public DateTimeOffset? ReminderAt { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
    public ICollection<TaskEvent> Events { get; set; } = new List<TaskEvent>();
}
