namespace Linework.Infrastructure;

public interface IClock
{
    DateTimeOffset Now { get; }
    DateOnly Today { get; }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTimeOffset.Now.DateTime);
}
