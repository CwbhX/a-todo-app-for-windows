using System.IO;

namespace Linework.Infrastructure;

public static class AppPaths
{
    public static string AppDataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Linework");

    public static string DatabasePath { get; } = Path.Combine(AppDataDirectory, "linework.db");

    public static string ExportsDirectory { get; } = Path.Combine(AppDataDirectory, "exports");

    public static string LogsDirectory { get; } = Path.Combine(AppDataDirectory, "logs");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(AppDataDirectory);
        Directory.CreateDirectory(ExportsDirectory);
        Directory.CreateDirectory(LogsDirectory);
    }
}
