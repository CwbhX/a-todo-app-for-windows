using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Linework.Infrastructure;

public sealed class DebugFileLoggerProvider : ILoggerProvider
{
    private readonly object gate = new();
    private readonly StreamWriter writer;
    private readonly LogLevel minimumLevel;
    private readonly IReadOnlyList<string> excludedCategoryPrefixes;

    public DebugFileLoggerProvider(
        string filePath,
        LogLevel minimumLevel = LogLevel.Debug,
        IReadOnlyList<string>? excludedCategoryPrefixes = null)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        writer = new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
        {
            AutoFlush = true
        };
        this.minimumLevel = minimumLevel;
        this.excludedCategoryPrefixes = excludedCategoryPrefixes ?? [];
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DebugFileLogger(categoryName, WriteLine, minimumLevel, excludedCategoryPrefixes);
    }

    public void Dispose()
    {
        lock (gate)
        {
            writer.Dispose();
        }
    }

    private void WriteLine(string line)
    {
        lock (gate)
        {
            writer.WriteLine(line);
        }
    }

    private sealed class DebugFileLogger(
        string categoryName,
        Action<string> writeLine,
        LogLevel minimumLevel,
        IReadOnlyList<string> excludedCategoryPrefixes) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None
                && logLevel >= minimumLevel
                && !excludedCategoryPrefixes.Any(prefix => categoryName.StartsWith(prefix, StringComparison.Ordinal));
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message) && exception is null)
            {
                return;
            }

            var timestamp = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz", CultureInfo.InvariantCulture);
            var eventText = eventId.Id == 0 && string.IsNullOrWhiteSpace(eventId.Name)
                ? string.Empty
                : $" [{eventId.Id}:{eventId.Name}]";
            writeLine($"{timestamp} {logLevel} {categoryName}{eventText}: {message}");

            if (exception is not null)
            {
                writeLine(exception.ToString());
            }
        }
    }
}
