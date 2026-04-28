namespace Linework.Services;

public interface IReminderService
{
    Task CheckDueRemindersAsync(CancellationToken ct = default);
}

public sealed class ReminderService : IReminderService
{
    public Task CheckDueRemindersAsync(CancellationToken ct = default)
    {
        // TODO Future phase: in-app reminders before any Windows notification work.
        return Task.CompletedTask;
    }
}
