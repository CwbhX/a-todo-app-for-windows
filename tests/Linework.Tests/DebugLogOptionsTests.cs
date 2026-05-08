using Linework.Infrastructure;
using Xunit;

namespace Linework.Tests;

public sealed class DebugLogOptionsTests
{
    [Fact]
    public void Parse_is_disabled_by_default()
    {
        var options = DebugLogOptions.Parse([]);

        Assert.False(options.IsEnabled);
        Assert.StartsWith(AppPaths.LogsDirectory, options.FilePath);
        Assert.Contains("linework-debug-", Path.GetFileName(options.FilePath));
    }

    [Fact]
    public void Parse_enables_debug_logging_with_flag()
    {
        var options = DebugLogOptions.Parse(["--debug-log"]);

        Assert.True(options.IsEnabled);
    }

    [Fact]
    public void Parse_resolves_bare_file_name_under_logs_directory()
    {
        var options = DebugLogOptions.Parse(["--debug-log", "--debug-log-file", "my-debug.log"]);

        Assert.True(options.IsEnabled);
        Assert.Equal(Path.Combine(AppPaths.LogsDirectory, "my-debug.log"), options.FilePath);
    }

    [Fact]
    public void Parse_preserves_rooted_path()
    {
        var rootedPath = Path.Combine(Path.GetTempPath(), "linework-rooted-debug.log");

        var options = DebugLogOptions.Parse(["--debug-log", "--debug-log-file", rootedPath]);

        Assert.Equal(rootedPath, options.FilePath);
    }

    [Fact]
    public void Parse_falls_back_when_file_value_is_missing()
    {
        var options = DebugLogOptions.Parse(["--debug-log", "--debug-log-file"]);

        Assert.True(options.IsEnabled);
        Assert.StartsWith(AppPaths.LogsDirectory, options.FilePath);
        Assert.Contains("linework-debug-", Path.GetFileName(options.FilePath));
    }

    [Fact]
    public void Parse_accepts_equals_style_file_value()
    {
        var options = DebugLogOptions.Parse(["--debug-log", "--debug-log-file=equals-debug.log"]);

        Assert.Equal(Path.Combine(AppPaths.LogsDirectory, "equals-debug.log"), options.FilePath);
    }
}
