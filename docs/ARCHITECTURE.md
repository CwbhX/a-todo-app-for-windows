# Architecture

Linework uses a simple WPF MVVM architecture:

```text
Views -> ViewModels -> Services -> Data/Models
```

## Projects

- `src/Linework.App`: WPF application, domain models, EF Core DbContext, services, and ViewModels.
- `tests/Linework.Tests`: xUnit tests using SQLite in-memory databases.

## Persistence

SQLite is accessed through `LineworkDbContext`. `DateTimeOffset` values are stored as UTC Unix milliseconds. `DateOnly` values are stored as ISO date strings.

The default database path is:

```text
%LOCALAPPDATA%\Linework\linework.db
```

## Services

- `TaskService` owns task lifecycle behavior and task events.
- `ProjectService` owns project creation/listing placeholders.
- `SearchService` starts with simple fallback search; FTS5 is deferred.
- `MarkdownService` wraps Markdig parsing; WPF-native rendering is deferred.
- `SettingsService` stores JSON values in `AppSetting`.
- Export, reminder, summary, and AI boundaries exist only as placeholders.

## UI

Phase 1 uses plain WPF shell controls. WPF-UI is referenced for later Fluent styling, but the shell keeps the dependency unused until it can be verified cleanly in the CLI build.
