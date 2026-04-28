# Linework Dev Diary

This diary is a long-running engineering log for Codex sessions. Keep it narrative and practical: what changed, why it changed, what was learned, what could not be verified, and what the next agent should pay attention to.

The formal source-of-truth docs remain:

- `AGENTS.md`
- `CODEX_STARTING_CONTEXT.md`
- `docs/PRODUCT_SPEC.md`
- `docs/ARCHITECTURE.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/DECISIONS.md`

Use this diary for session history and handoff context, not as a replacement for those docs.

---

## 2026-04-27 - Phase 1 Foundation And Rename To Linework

### Starting Point

The repo was effectively blank except for agent/project context:

- `AGENTS.md`
- `dev-docs/CODEX_STARTING_CONTEXT.md`

Both documents still described the app as `WorkDone`. The user clarified that the app is now named `Linework` and asked for Phase 1 only: solution setup, WPF app project, xUnit test project, basic repo structure, placeholder WPF shell, initial MVVM ViewModels, initial domain models, EF Core SQLite DbContext, service interfaces/placeholders, starter docs, and practical TaskService tests.

The user also explicitly said not to implement AI summaries, sync, recurring tasks, system tray, OS notifications, full search, full markdown rendering, or production UI yet.

### Context Read

Read:

- `AGENTS.md`
- `dev-docs/CODEX_STARTING_CONTEXT.md`

`CODEX_STARTING_CONTEXT.md` was not present at the repo root at the start, but it existed under `dev-docs/`. The repo now has a root `CODEX_STARTING_CONTEXT.md` plus `docs/CODEX_STARTING_CONTEXT.md` with Linework-specific condensed context.

### Naming Decisions

Applied the rename consistently:

- Solution: `Linework`
- App project: `Linework.App`
- Test project: `Linework.Tests`
- Root namespace: `Linework`
- App display name: `Linework`
- Local data folder: `%LOCALAPPDATA%\Linework\`
- Database file: `linework.db`

Why: stale naming creates ongoing friction for agents, docs, namespaces, paths, and future packaging. The first foundation pass is the lowest-cost time to make the rename complete.

### Files Created

Created the initial solution and repo shape:

- `Linework.sln`
- `.gitignore`
- `README.md`
- `CODEX_STARTING_CONTEXT.md`
- `docs/CODEX_STARTING_CONTEXT.md`
- `docs/PRODUCT_SPEC.md`
- `docs/ARCHITECTURE.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/DECISIONS.md`

Created the WPF app project:

- `src/Linework.App/Linework.App.csproj`
- `src/Linework.App/App.xaml`
- `src/Linework.App/App.xaml.cs`
- `src/Linework.App/MainWindow.xaml`
- `src/Linework.App/MainWindow.xaml.cs`

Created placeholder views:

- `src/Linework.App/Views/TodayView.xaml`
- `src/Linework.App/Views/ActiveTasksView.xaml`
- `src/Linework.App/Views/DoneView.xaml`
- `src/Linework.App/Views/ProjectsView.xaml`
- `src/Linework.App/Views/SearchView.xaml`
- `src/Linework.App/Views/SettingsView.xaml`
- `src/Linework.App/Views/TaskDetailsView.xaml`

Created initial ViewModels:

- `src/Linework.App/ViewModels/MainViewModel.cs`
- `src/Linework.App/ViewModels/TodayViewModel.cs`
- `src/Linework.App/ViewModels/TaskListViewModel.cs`
- `src/Linework.App/ViewModels/TaskDetailsViewModel.cs`
- `src/Linework.App/ViewModels/ProjectsViewModel.cs`
- `src/Linework.App/ViewModels/SearchViewModel.cs`
- `src/Linework.App/ViewModels/SettingsViewModel.cs`

Created initial models:

- `src/Linework.App/Models/TaskItem.cs`
- `src/Linework.App/Models/TaskEvent.cs`
- `src/Linework.App/Models/Project.cs`
- `src/Linework.App/Models/AppSetting.cs`
- `src/Linework.App/Models/Enums.cs`

Created persistence layer:

- `src/Linework.App/Data/LineworkDbContext.cs`
- `src/Linework.App/Data/DbInitializer.cs`
- `src/Linework.App/Data/Converters/DateTimeOffsetUnixMillisecondsConverter.cs`
- `src/Linework.App/Data/Converters/NullableDateTimeOffsetUnixMillisecondsConverter.cs`
- `src/Linework.App/Data/Converters/NullableDateOnlyIsoStringConverter.cs`

Created infrastructure:

- `src/Linework.App/Infrastructure/AppPaths.cs`
- `src/Linework.App/Infrastructure/Clock.cs`
- `src/Linework.App/Infrastructure/UrlOpener.cs`

Created service boundaries and minimal implementations:

- `src/Linework.App/Services/TaskService.cs`
- `src/Linework.App/Services/ProjectService.cs`
- `src/Linework.App/Services/SearchService.cs`
- `src/Linework.App/Services/SearchIndexService.cs`
- `src/Linework.App/Services/MarkdownService.cs`
- `src/Linework.App/Services/SummaryService.cs`
- `src/Linework.App/Services/ExportService.cs`
- `src/Linework.App/Services/ReminderService.cs`
- `src/Linework.App/Services/SettingsService.cs`
- `src/Linework.App/Services/Ai/IAiSummaryProvider.cs`

Created the test project:

- `tests/Linework.Tests/Linework.Tests.csproj`
- `tests/Linework.Tests/TestDbFactory.cs`
- `tests/Linework.Tests/FixedClock.cs`
- `tests/Linework.Tests/TaskServiceTests.cs`

### Files Updated

Updated:

- `AGENTS.md`
- `dev-docs/CODEX_STARTING_CONTEXT.md`

Why: both still used the old `WorkDone` name. The user explicitly requested stale references to be updated across repo and docs.

### Packages Added

App project:

- `CommunityToolkit.Mvvm` 8.4.2
- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.6
- `Microsoft.EntityFrameworkCore.Design` 10.0.6
- `Microsoft.Extensions.Hosting` 10.0.6
- `Markdig` 0.41.3
- `WPF-UI` 4.2.0

Test project:

- `Microsoft.EntityFrameworkCore.Sqlite` 10.0.6
- `Microsoft.NET.Test.Sdk` 18.4.0
- `xunit.v3` 3.2.2
- `xunit.runner.visualstudio` 3.1.5

Why:

- CommunityToolkit.Mvvm matches the project MVVM requirement.
- EF Core SQLite gives durable local persistence from day one.
- EF Core Design supports future migrations.
- Microsoft.Extensions.Hosting gives simple DI and app startup composition.
- Markdig is the required markdown parser.
- WPF-UI is referenced for later Fluent-ish styling, but not yet used in the XAML shell.
- xUnit and SQLite in-memory support service/data tests without fragile WPF UI automation.

### Architecture Choices

Kept the app as one WPF project plus one test project.

Why: the repo instructions explicitly avoid overbuilt clean architecture and separate class libraries for v1. The goal is easy iteration and a codebase that agents can quickly understand.

Used this simple layering:

```text
Views -> ViewModels -> Services -> Data/Models
```

Why: it keeps WPF UI code thin, keeps business behavior out of ViewModels, and gives future phases clear places to add task behavior, projects, search, markdown, and settings.

Added `LineworkDbContext` as the EF Core boundary.

Why: persistence should be centralized and testable. The app will eventually depend on event history and reliable completed-work data.

Added `IClock` and `SystemClock`.

Why: done grace-period behavior and timestamps are important enough to test deterministically.

Added `AppPaths`.

Why: the local data path should be owned in one place and should use `%LOCALAPPDATA%\Linework\linework.db`, not a repo-local file.

### Domain Choices

Added core entities:

- `TaskItem`
- `TaskEvent`
- `Project`
- `AppSetting`

Added task statuses:

- `Active`
- `Done`
- `Archived`

Added task event types:

- `Created`
- `Edited`
- `MarkedDone`
- `Reopened`
- `Archived`
- plus future-facing change event types for dates, project, and importance.

Why: task completion should preserve history. Future weekly/monthly summaries need more than current task state.

### Persistence Choices

Configured SQLite via EF Core.

Configured DateTimeOffset converters to persist UTC Unix milliseconds.

Configured nullable DateOnly converter to persist ISO `yyyy-MM-dd` strings.

Added indexes for:

- task status
- project id
- planned date
- due date
- completed date
- archived date
- task event task id
- task event occurrence time
- project name

Why: the starting context warned that SQLite does not have native `DateTimeOffset` semantics that should be trusted blindly. The converter approach keeps storage predictable.

### UI Choices

Created a basic plain WPF shell:

- left navigation
- central placeholder content
- right details placeholder

Why: Phase 1 needs a buildable shell, not a production UI. The shell makes the future app shape visible without implementing CRUD screens too early.

Referenced WPF-UI but did not use WPF-UI controls yet.

Why: WPF-UI is desired, but package restore/build could not be verified in this environment. Plain WPF keeps the shell conservative until the SDK is available.

### TaskService Behavior Added

Implemented minimal lifecycle methods:

- create task
- get task
- query today
- query active
- query done
- mark done
- reopen
- archive

Important behavior included:

- creating a task records a `Created` event
- marking done sets `Status = Done`, `CompletedAt`, `UpdatedAt`, and records `MarkedDone`
- reopening sets `Status = Active`, clears `CompletedAt`, clears `ArchivedAt`, and records `Reopened`
- archiving sets `Status = Archived`, sets `ArchivedAt`, and records `Archived`
- active query includes active tasks plus tasks completed inside a one-day grace period
- active query excludes archived tasks
- done query includes completed tasks

Why: these are the core lifecycle rules in the product spec, and they are good early service tests because they avoid fragile UI automation.

### Tests Added

Added SQLite in-memory tests for:

- create task records `Created`
- mark done updates state and records `MarkedDone`
- reopen clears completion state and records `Reopened`
- archive excludes from active query and records `Archived`
- active query includes recently completed tasks inside grace period
- active query hides completed tasks after grace period

Why: SQLite behavior matters for this app, so EF Core InMemory would be a weaker test target.

### Verification Attempted

Tried to run:

```powershell
dotnet restore
dotnet build
dotnet test
```

All three failed because `dotnet` is not available in this environment:

```text
dotnet : The term 'dotnet' is not recognized as the name of a cmdlet, function, script file, or operable program.
```

An escalated restore attempt was also made, but it failed with the same error.

This means the repo has not yet been compiler-verified. The next session should begin by installing/exposing the .NET 10 SDK and running restore/build/test before adding new behavior.

### Stale Name Sweep

Searched for stale names:

- `WorkDone`
- `workdone`
- `WORKDONE`
- `Work Done`
- `LineWork`
- `Line Work`

Final case-sensitive search found no matches.

### Known Risks

Because the .NET SDK was unavailable, there may be compile issues that only show up once restore/build runs. Areas to check first:

- xUnit v3 package/test runner behavior with `dotnet test`
- WPF project targeting `net10.0-windows`
- WPF-UI package compatibility after restore
- source-generated CommunityToolkit.Mvvm code
- XAML namespace/code-behind generation
- EF Core value converters and SQLite model creation

### Intentional TODOs

Deferred intentionally:

- real task list UI
- quick add
- task details editing
- project assignment UI
- markdown WPF-native preview
- SQLite FTS5 setup
- search index syncing
- export
- settings defaults
- theme integration
- migrations
- AI provider implementations
- Windows notifications
- recurring tasks
- sync
- tray behavior

### PR Draft From This Session

Suggested PR title:

```text
Initialize Linework WPF foundation
```

Suggested PR body:

```md
## Summary
- Rename project/docs from WorkDone to Linework
- Add initial .NET 10 WPF solution with `Linework.App` and `Linework.Tests`
- Add placeholder shell UI, MVVM ViewModels, domain models, EF Core SQLite DbContext, infrastructure, and service boundaries
- Add basic `TaskService` lifecycle behavior and SQLite in-memory tests
- Add starter docs for product spec, architecture, implementation plan, and decisions

## Verification
- Searched for stale names: `WorkDone`, `workdone`, `WORKDONE`, `Work Done`, `LineWork`, `Line Work`
- `dotnet restore`, `dotnet build`, and `dotnet test` could not run because `dotnet` is not available on PATH in the current environment

## Notes
- WPF-UI is referenced but the shell currently uses plain WPF until restore/build can verify it cleanly
- This is Phase 1 foundation only; no AI, sync, recurring tasks, tray, OS notifications, full search, full markdown rendering, or production UI
```

### Recommended Next Prompt

```text
Phase 2: First, make sure the .NET 10 SDK is installed and available on PATH. Run dotnet restore, dotnet build, and dotnet test. Fix any compile/test issues from the Phase 1 foundation. Then harden the database layer: decide whether to use migrations now or keep EnsureCreated temporarily, add settings defaults, verify date/time converters with tests, and add ProjectService persistence tests. Keep the scope small and do not implement the full UI yet.
```
