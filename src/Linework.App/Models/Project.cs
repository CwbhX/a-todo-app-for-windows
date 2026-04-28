namespace Linework.Models;

public sealed class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ColorKey { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
