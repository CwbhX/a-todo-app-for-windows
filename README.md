# Linework

Linework is a Windows-native, local-first personal task and done-history app. It starts as a simple WPF todo/planner shell, then grows into a lightweight ledger of completed work over time.

## Current Status

Phase 1 foundation:

- .NET 10 WPF app project
- xUnit test project
- MVVM shell placeholders
- EF Core SQLite DbContext and domain models
- Basic task lifecycle service
- Starter service boundaries for future markdown, search, settings, export, reminders, and summaries

## Commands

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Linework.App
```

To capture debug logs for a local run:

```powershell
dotnet run --project src/Linework.App -- --debug-log
dotnet run --project src/Linework.App -- --debug-log --debug-log-file smoke-debug.log
```

Bare debug log file names are written under `%LOCALAPPDATA%\Linework\logs`. Rooted paths are accepted when you want the log somewhere specific.

Linework stores local data under:

```text
%LOCALAPPDATA%\Linework\linework.db
```

## Notes

The first implementation is intentionally small. It does not include sync, AI summaries, recurring tasks, system tray behavior, Windows notifications, full search, or full markdown rendering.
