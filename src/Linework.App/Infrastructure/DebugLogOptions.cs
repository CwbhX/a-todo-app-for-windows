using System.IO;

namespace Linework.Infrastructure;

public sealed record DebugLogOptions(bool IsEnabled, string FilePath)
{
    private const string DefaultFileNamePrefix = "linework-debug";

    public static DebugLogOptions Parse(IEnumerable<string> args)
    {
        var values = args.ToArray();
        var isEnabled = values.Any(arg => string.Equals(arg, "--debug-log", StringComparison.OrdinalIgnoreCase));
        var fileNameOrPath = FindOptionValue(values, "--debug-log-file");
        var filePath = ResolveFilePath(fileNameOrPath);

        return new DebugLogOptions(isEnabled, filePath);
    }

    private static string? FindOptionValue(IReadOnlyList<string> args, string optionName)
    {
        for (var index = 0; index < args.Count; index++)
        {
            var arg = args[index];
            if (string.Equals(arg, optionName, StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 >= args.Count || args[index + 1].StartsWith("--", StringComparison.Ordinal))
                {
                    return null;
                }

                return args[index + 1];
            }

            var prefix = optionName + "=";
            if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return arg[prefix.Length..];
            }
        }

        return null;
    }

    private static string ResolveFilePath(string? fileNameOrPath)
    {
        var candidate = string.IsNullOrWhiteSpace(fileNameOrPath)
            ? DefaultFileName()
            : fileNameOrPath.Trim();

        if (HasInvalidFileName(candidate))
        {
            candidate = DefaultFileName();
        }

        return Path.IsPathRooted(candidate)
            ? candidate
            : Path.Combine(AppPaths.LogsDirectory, candidate);
    }

    private static bool HasInvalidFileName(string value)
    {
        var fileName = Path.GetFileName(value);
        return fileName.Length == 0 || fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0;
    }

    private static string DefaultFileName()
    {
        return $"{DefaultFileNamePrefix}-{DateTimeOffset.Now:yyyyMMdd-HHmmss}.log";
    }
}
