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

The app currently uses `EnsureCreated` during startup. This is intentional while the schema is still in the foundation phase. Initial EF migrations should be introduced before a build with real user data is distributed.

First-run database initialization also seeds default settings:

- theme mode: system
- default view: Today
- done grace period: 1 day

## Services

- `TaskService` owns task lifecycle behavior and task events. It reads `tasks.doneGracePeriodDays` through `SettingsService` when querying Active tasks, falling back to one day if the setting is missing.
- `ProjectService` owns project creation/listing placeholders.
- `SearchService` starts with simple fallback search; FTS5 is deferred.
- `MarkdownService` wraps Markdig parsing; WPF-native rendering is deferred.
- `SettingsService` stores JSON values in `AppSetting` and uses readable enum strings.
- Export, reminder, summary, and AI boundaries exist only as placeholders.

## UI

The app currently uses plain WPF shell controls. WPF-UI is referenced for later Fluent styling, but the shell keeps the dependency unused.

The first persisted UI slice lives in `MainViewModel` and `MainWindow`: quick-add for Today/Active, persisted task rows for Today/Active/Done, a row-level Plan today action for unplanned Active tasks, and a mark-done button. Code-behind remains limited to initial async loading.
