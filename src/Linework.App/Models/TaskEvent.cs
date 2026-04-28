namespace Linework.Models;

public sealed class TaskEvent
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
    public TaskEventType EventType { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string? PayloadJson { get; set; }
}
