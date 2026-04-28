using Linework.Infrastructure;

namespace Linework.Tests;

public sealed class FixedClock(DateTimeOffset now) : IClock
{
    public DateTimeOffset Now { get; set; } = now;

    public DateOnly Today => DateOnly.FromDateTime(Now.LocalDateTime);
}
