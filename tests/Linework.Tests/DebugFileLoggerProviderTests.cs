using System.Text.RegularExpressions;
using Linework.Infrastructure;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Linework.Tests;

public sealed class DebugFileLoggerProviderTests
{
    [Fact]
    public void Logger_writes_timestamped_log_lines()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Linework.Tests", Guid.NewGuid().ToString("N"));
        var filePath = Path.Combine(directory, "nested", "debug.log");

        using (var provider = new DebugFileLoggerProvider(filePath))
        {
            var logger = provider.CreateLogger("Linework.Tests.Debug");

            logger.LogDebug("Feature {FeatureName} landed.", "debug logging");
        }

        var lines = File.ReadAllLines(filePath);

        Assert.Single(lines);
        Assert.Matches(
            new Regex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}[-+]\d{2}:\d{2} Debug Linework\.Tests\.Debug: Feature debug logging landed\.$"),
            lines[0]);
    }

    [Fact]
    public void Logger_writes_exception_details()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Linework.Tests", Guid.NewGuid().ToString("N"));
        var filePath = Path.Combine(directory, "debug.log");
        var exception = new InvalidOperationException("Something went sideways.");

        using (var provider = new DebugFileLoggerProvider(filePath))
        {
            var logger = provider.CreateLogger("Linework.Tests.Debug");

            logger.LogError(exception, "Debug failure.");
        }

        var contents = File.ReadAllText(filePath);

        Assert.Contains("Error Linework.Tests.Debug: Debug failure.", contents);
        Assert.Contains("System.InvalidOperationException: Something went sideways.", contents);
    }

    [Fact]
    public void Logger_creates_parent_directory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "Linework.Tests", Guid.NewGuid().ToString("N"));
        var filePath = Path.Combine(directory, "deep", "debug.log");

        using (var provider = new DebugFileLoggerProvider(filePath))
        {
            provider.CreateLogger("Linework.Tests.Debug").LogInformation("Created.");
        }

        Assert.True(File.Exists(filePath));
    }
}
