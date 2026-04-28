# Linework - Codex Starting Context

This document is the canonical starting context for Codex. Use it before making architectural decisions. The goal is to build a simple, native Windows app that helps track both active work and completed work over time.

Place this file at:

```text
/docs/CODEX_STARTING_CONTEXT.md
```

Suggested app/repo name: `Linework`. If the app is renamed later, keep the architecture and product intent the same.

---

## 1. Product intent

`Linework` is a local-first Windows-native personal task and done-history app.

It is not just a generic todo list. It should feel like a lightweight personal work ledger:

```text
What do I need to do?
What am I trying to get done today?
What did I finish?
What did I accomplish this week or month?
```

The first version should be very simple and fast to build. Long-term, the app should support weekly/monthly summaries, optional AI-generated summaries, and smart review/reprioritization. Do not build those advanced features in the first revision unless explicitly requested later.

---

## 2. User requirements captured so far

These are the current product decisions from the user.

### Main use cases

The app should track:

- Engineering/work tasks.
- Life/admin tasks.
- Small reminders.
- Things the user wants to do today.
- Things the user has completed over time.

### Product style

The app should be a hybrid:

- Clean todo app.
- Work accomplishment journal.
- Done-history ledger.

### Important user preferences

- Must be Windows native.
- Must not be Electron.
- Must not use a Chromium app shell.
- Should look modern, roughly Fluent/Windows 11-ish.
- Should support system light/dark mode.
- Should be buildable and runnable from CLI with Codex, ideally without requiring Visual Studio as the daily development tool.
- Visual Studio can be installed as a fallback for debugging/XAML help, but it should not be required for normal build/test/run.
- Codex should own almost all implementation, but it must work in small, verified phases.

### V1 feature decisions

| Area | Decision |
|---|---|
| Due dates | Support due/reminder fields in the data model. OS notifications can come later. |
| Priority | Start with a simple `Important` star. Keep data flexible enough for Eisenhower-style views later. |
| Projects | Optional project per task. No required project. |
| Subtasks | Do not implement real subtasks in v1. Support markdown checklist syntax in notes. |
| Mark done behavior | When marked done, keep visible and crossed out for a grace period, then hide from active views. Always keep in Done history. |
| Done grace period | Default to 1 day. Make it configurable later. |
| Delete behavior | Archive by default, not hard delete. |
| Time tracking | Skip. |
| Today view | Yes. It should show planned work and completed work for today. |
| Recurring tasks | No. |
| End-of-day prompt | Future feature: prompt user to review what was finished. |
| System tray | Not v1. Maybe later. |
| Markdown notes | Yes. Support markdown simply, with preview. |
| Links | Support simple web links in markdown and tasks. Links open in default browser. |
| Search | Search should be smart and include active/done/title/notes/projects. |
| Completed tasks editable | Yes. |
| Sync | Local-first. Export/backup matters. No sync in v1. |
| Export | Markdown export later if useful. |
| AI summaries | Later. Must support generic OpenAI-compatible APIs and local Ollama-style usage. |
| External tools | No GitHub/Jira/Slack/email integration for now. |

---

## 3. Architecture decision

Use this stack:

```text
Language: C#
Runtime: .NET 10 LTS
UI: WPF
UI polish: WPF UI package
Pattern: MVVM
MVVM helper: CommunityToolkit.Mvvm
Database: SQLite
Data access: EF Core plus raw SQL for FTS/search where needed
Markdown parser: Markdig
Search: SQLite FTS5
Tests: xUnit with SQLite in-memory databases
```

### Why WPF instead of WinUI 3?

WinUI 3 is a modern Microsoft UI option, but WPF is better for this first revision because:

- It is mature and well understood by coding agents.
- It is Windows-native and does not require Electron or Chromium.
- It builds cleanly from the `dotnet` CLI.
- It has straightforward MVVM patterns.
- WPF UI can give it a modern Fluent-ish look without requiring a full WinUI app architecture.
- The app is simple enough that WPF is more than sufficient.

### Why SQLite?

SQLite gives the app a real durable local database from day one. This matters because the app is not only a todo list. It needs reliable history for future weekly/monthly summaries.

### Why task events?

Do not only store current task state. Also store a lightweight event history.

This makes future summaries much better because the app can answer questions like:

```text
What did I complete this week?
What tasks were reopened?
What projects did I touch?
Which active tasks have gone stale?
```

---

## 4. Non-goals for v1

Do not implement these in the first revision:

- AI summaries.
- Recurring tasks.
- System tray behavior.
- Global hotkey quick-add.
- Mobile companion app.
- Cloud sync.
- External integrations.
- Full Eisenhower board.
- Full notification system.
- Complex rich text editor.
- WebView2 or browser-based markdown rendering.
- Electron.
- A giant clean architecture setup with many projects.

It is acceptable to leave small interfaces/stubs for future features, but do not let future features slow down v1.

---

## 5. Recommended repo structure

Keep the app simple. Use one WPF app project and one test project.

```text
Linework/
  README.md
  Linework.sln

  docs/
    CODEX_STARTING_CONTEXT.md
    PRODUCT_SPEC.md
    ARCHITECTURE.md
    IMPLEMENTATION_PLAN.md
    DECISIONS.md

  src/
    Linework.App/
      Linework.App.csproj
      App.xaml
      App.xaml.cs
      MainWindow.xaml
      MainWindow.xaml.cs

      Views/
        TodayView.xaml
        ActiveTasksView.xaml
        DoneView.xaml
        ProjectsView.xaml
        SearchView.xaml
        TaskDetailsView.xaml
        SettingsView.xaml

      ViewModels/
        MainViewModel.cs
        TodayViewModel.cs
        TaskListViewModel.cs
        TaskDetailsViewModel.cs
        ProjectsViewModel.cs
        SearchViewModel.cs
        SettingsViewModel.cs

      Models/
        TaskItem.cs
        Project.cs
        TaskEvent.cs
        AppSetting.cs
        Enums.cs

      Data/
        LineworkDbContext.cs
        DbInitializer.cs
        Migrations/
        Converters/
          DateTimeOffsetUnixMillisecondsConverter.cs
          NullableDateTimeOffsetUnixMillisecondsConverter.cs
          DateOnlyIsoStringConverter.cs

      Services/
        TaskService.cs
        ProjectService.cs
        SearchService.cs
        SearchIndexService.cs
        MarkdownService.cs
        SummaryService.cs
        ExportService.cs
        ReminderService.cs
        SettingsService.cs
        Ai/
          IAiSummaryProvider.cs
          OpenAICompatibleSummaryProvider.cs
          OllamaNativeSummaryProvider.cs

      Infrastructure/
        AppPaths.cs
        Clock.cs
        UrlOpener.cs
        Result.cs

  tests/
    Linework.Tests/
      Linework.Tests.csproj
      TaskServiceTests.cs
      ProjectServiceTests.cs
      SearchServiceTests.cs
      SummaryServiceTests.cs
      MarkdownServiceTests.cs
      TestDbFactory.cs
```

---

## 6. Initial CLI setup

Target Windows. The project should build from PowerShell using the .NET SDK.

```powershell
dotnet new sln -n Linework

dotnet new wpf -n Linework.App -o src/Linework.App

dotnet sln add src/Linework.App/Linework.App.csproj

dotnet add src/Linework.App package CommunityToolkit.Mvvm
dotnet add src/Linework.App package Microsoft.EntityFrameworkCore.Sqlite
dotnet add src/Linework.App package Microsoft.EntityFrameworkCore.Design
dotnet add src/Linework.App package WPF-UI
dotnet add src/Linework.App package Markdig

dotnet new xunit -n Linework.Tests -o tests/Linework.Tests
dotnet sln add tests/Linework.Tests/Linework.Tests.csproj
dotnet add tests/Linework.Tests reference src/Linework.App/Linework.App.csproj
dotnet add tests/Linework.Tests package Microsoft.EntityFrameworkCore.Sqlite

dotnet build
dotnet test
```

If EF migrations are used from CLI:

```powershell
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate --project src/Linework.App --startup-project src/Linework.App

dotnet ef database update --project src/Linework.App --startup-project src/Linework.App
```

The WPF project should target Windows explicitly:

```xml
<TargetFramework>net10.0-windows</TargetFramework>
<UseWPF>true</UseWPF>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

---

## 7. App data location

Use a local app data folder:

```text
%LOCALAPPDATA%\Linework\linework.db
%LOCALAPPDATA%\Linework\logs\
%LOCALAPPDATA%\Linework\exports\
```

`AppPaths` should own all paths.

```csharp
public static class AppPaths
{
    public static string AppDataDirectory { get; }
    public static string DatabasePath { get; }
    public static string ExportsDirectory { get; }
}
```

Create directories on startup.

---

## 8. Domain model

### Task status

```csharp
public enum TaskItemStatus
{
    Active = 0,
    Done = 1,
    Archived = 2
}
```

### Task event type

```csharp
public enum TaskEventType
{
    Created = 0,
    Edited = 1,
    MarkedDone = 2,
    Reopened = 3,
    Archived = 4,
    PlannedForDateChanged = 5,
    DueDateChanged = 6,
    ReminderChanged = 7,
    ProjectChanged = 8,
    ImportantChanged = 9
}
```

### TaskItem

Use `Guid` IDs so data remains export/import-friendly later.

```csharp
public sealed class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? MarkdownNotes { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Active;
    public bool IsImportant { get; set; }

    public Guid? ProjectId { get; set; }
    public Project? Project { get; set; }

    public DateOnly? PlannedForDate { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public DateTimeOffset? ReminderAt { get; set; }

    public int SortOrder { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }
}
```

### Project

```csharp
public sealed class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ColorKey { get; set; }
    public int SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ArchivedAt { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
```

### TaskEvent

```csharp
public sealed class TaskEvent
{
    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;

    public TaskEventType EventType { get; set; }
    public DateTimeOffset OccurredAt { get; set; }

    // Small JSON payload for before/after values, optional completion note later, etc.
    public string? PayloadJson { get; set; }
}
```

### AppSetting

```csharp
public sealed class AppSetting
{
    public string Key { get; set; } = string.Empty;
    public string ValueJson { get; set; } = string.Empty;
}
```

---

## 9. Date/time persistence rules

SQLite does not have a native `DateTimeOffset` type with full server-side comparison semantics through EF Core. Do not let date behavior become fragile.

Recommended approach:

- Domain model can use `DateTimeOffset` and `DateOnly`.
- Persist `DateTimeOffset` as Unix milliseconds in an INTEGER column via EF value converters.
- Persist `DateOnly` as ISO date string `yyyy-MM-dd` via an EF value converter.
- Use an `IClock`/`Clock` abstraction so tests can use fixed time.
- Store instants in UTC.
- Convert to local time only in the UI.

Example converter direction:

```csharp
public sealed class DateTimeOffsetUnixMillisecondsConverter
    : ValueConverter<DateTimeOffset, long>
{
    public DateTimeOffsetUnixMillisecondsConverter()
        : base(
            value => value.ToUniversalTime().ToUnixTimeMilliseconds(),
            value => DateTimeOffset.FromUnixTimeMilliseconds(value))
    {
    }
}
```

For nullable values, create a nullable converter.

---

## 10. Database indexes

Minimum indexes:

```text
TaskItem.Status
TaskItem.ProjectId
TaskItem.PlannedForDate
TaskItem.DueAt
TaskItem.CompletedAt
TaskItem.ArchivedAt
TaskEvent.TaskItemId
TaskEvent.OccurredAt
Project.Name
```

Sort rules:

1. Important tasks first.
2. Planned-for-today tasks near top.
3. Due soon tasks near top.
4. Manual `SortOrder` within groups.
5. Newer/updated tasks if no sort order exists.

---

## 11. Done grace period behavior

When a task is marked done:

- Set `Status = Done`.
- Set `CompletedAt = clock.Now`.
- Set `UpdatedAt = clock.Now`.
- Add `TaskEvent` with `EventType = MarkedDone`.
- Keep it visible and crossed out in Today/Active while it is inside the done grace period.
- Hide it from Today/Active after the grace period.
- Always show it in the Done view.

Default grace period:

```text
1 day
```

Active query should include:

```text
Status == Active
OR
Status == Done AND CompletedAt >= Now - DoneGracePeriod
```

UI should render recently completed tasks with:

- Strikethrough title.
- Muted opacity.
- Completed timestamp.
- Reopen action.

Do not hard-delete done tasks.

---

## 12. Today view

Today is the default/home view.

It should have at least two sections:

```text
Planned for today
- Active tasks where PlannedForDate == today
- Active tasks due today
- Important active tasks can be shown near top

Done today
- Tasks completed today
```

Later, Today can also show:

- Stale tasks.
- End-of-day review prompt.
- AI suggested priorities.

But v1 should stay simple.

---

## 13. Active view

Shows active tasks plus recently completed tasks inside the done grace period.

Suggested filters:

- All active.
- Important.
- Due soon.
- By project.

Do not build complex filters until core CRUD is stable.

---

## 14. Done view

Done view should preserve accomplishment history.

Suggested grouping:

```text
Today
Yesterday
This week
Earlier
```

Each done task should show:

- Title.
- Project, if any.
- Completed time/date.
- Notes snippet, if useful.
- Reopen action.

Completed tasks should remain editable.

---

## 15. Projects

Projects are optional.

Examples:

```text
Work
Personal
Errands
OpenClaw
Ohmly
Apartment
```

V1 project features:

- Create project.
- Rename project.
- Archive project.
- Assign task to project.
- Filter tasks by project.

Do not require every task to have a project.

---

## 16. Markdown notes

The user wants markdown support, but it should stay simple.

V1 requirements:

- Store notes as raw markdown text.
- Show editor and preview.
- Support a useful subset:
  - Paragraphs.
  - Headings.
  - Bullets.
  - Numbered lists.
  - Task-list checkboxes as display-only initially.
  - Bold/italic.
  - Inline code.
  - Code blocks.
  - Links.
- Links open in the default browser.

Important constraint:

```text
Do not use WebView2 for markdown preview in v1.
Do not render markdown by embedding a browser.
```

Use Markdig to parse markdown, then render a practical subset into WPF UI elements or a WPF `FlowDocument`. If this is too much for the first pass, implement raw markdown editing first and a simple preview for headings/lists/links next.

The app should not require HTML rendering.

---

## 17. Opening links

Use the default browser:

```csharp
public sealed class UrlOpener
{
    public void Open(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
```

Only open `http://` and `https://` links automatically. Treat other schemes conservatively.

---

## 18. Smart search

Search should cover:

- Active tasks.
- Done tasks.
- Archived tasks only when explicitly included.
- Titles.
- Markdown notes.
- Project names.

Use SQLite FTS5 for real full-text search.

Possible FTS table:

```sql
CREATE VIRTUAL TABLE task_search USING fts5(
    task_id UNINDEXED,
    title,
    notes,
    project_name,
    status UNINDEXED,
    completed_at_unix_ms UNINDEXED,
    tokenize = 'unicode61'
);
```

Recommended implementation:

- `SearchIndexService` owns syncing task/project data into FTS.
- Update the FTS index whenever a task or project changes.
- Provide a full rebuild function for safety.
- If FTS setup fails, fallback to a simple `LIKE` search and log the issue.

Search grammar can start simple:

```text
text search across title/notes/project
```

Future query examples:

```text
is:done
is:active
project:OpenClaw
important:true
due:today
done:this-week
```

Do not build the full grammar before core search works.

---

## 19. Settings

Settings should be stored locally in `AppSetting` as JSON values.

Initial settings:

```text
ThemeMode: System | Light | Dark
DefaultView: Today
DoneGracePeriod: 1 day
EndOfDayPromptTime: 8:30 PM, future feature
AiProvider: None, future feature
AiBaseUrl: optional, future feature
AiModel: optional, future feature
```

Theme mode should default to system.

---

## 20. Service layer

Do not put core logic in ViewModels. ViewModels should call services.

### TaskService

Responsibilities:

- Create task.
- Update task title/notes/project/dates/importance.
- Mark done.
- Reopen.
- Archive.
- Query Today/Active/Done.
- Record task events.
- Trigger search index update.

Suggested interface:

```csharp
public interface ITaskService
{
    Task<TaskItem> CreateTaskAsync(CreateTaskRequest request, CancellationToken ct = default);
    Task<TaskItem?> GetTaskAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetTodayTasksAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetActiveTasksAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetDoneTasksAsync(DoneQuery query, CancellationToken ct = default);
    Task MarkDoneAsync(Guid taskId, CancellationToken ct = default);
    Task ReopenAsync(Guid taskId, CancellationToken ct = default);
    Task ArchiveAsync(Guid taskId, CancellationToken ct = default);
}
```

### ProjectService

Responsibilities:

- Create project.
- Rename project.
- Archive project.
- List active projects.

### SearchService

Responsibilities:

- Search tasks.
- Parse basic filters.
- Use FTS5 where available.
- Fallback safely.

### MarkdownService

Responsibilities:

- Parse markdown.
- Render preview model/FlowDocument.
- Extract links.
- Sanitize supported link schemes.

### SummaryService

Responsibilities:

- Generate deterministic weekly/monthly summary data without AI.
- This is a future-facing service. It can be a skeleton in v1.

### ExportService

Responsibilities:

- Export completed tasks to markdown.
- Later export JSON backup.

### ReminderService

Responsibilities:

- Later: end-of-day prompt.
- Later: due/reminder notifications.
- In v1, do not implement OS notifications.

---

## 21. AI summary architecture for later

Do not build AI in v1, but design so it can be added cleanly.

The AI feature should use structured local summary data, not raw database dumps.

Example summary input:

```json
{
  "periodStart": "2026-04-20",
  "periodEnd": "2026-04-26",
  "completedTasks": [
    {
      "title": "Fix build script",
      "project": "OpenClaw",
      "completedAt": "2026-04-21T18:10:00Z",
      "notes": "Short markdown notes if present"
    }
  ],
  "activeCarryoverTasks": [],
  "staleTasks": [],
  "projectsTouched": ["OpenClaw", "Personal"]
}
```

Recommended interface:

```csharp
public interface IAiSummaryProvider
{
    Task<string> GenerateSummaryAsync(SummaryInput input, CancellationToken ct = default);
}
```

Provider implementations later:

```text
OpenAICompatibleSummaryProvider
- BaseUrl
- ApiKey
- Model
- EndpointKind: ChatCompletions or Responses

OllamaNativeSummaryProvider
- BaseUrl, usually http://localhost:11434
- Model
- Native /api/chat style call
```

Privacy rules:

- Never send task data to an AI provider automatically.
- Require explicit user action.
- Show which provider/model will be used.
- Make AI disabled by default.
- Store API keys carefully. Prefer Windows Credential Manager or DPAPI later. Do not store plain API keys in the SQLite settings table.

---

## 22. UI layout

Use a single main window with a shell layout.

```text
MainWindow
  Left navigation
    Today
    Active
    Done
    Projects
    Search
    Settings

  Center content
    Current view

  Right details panel
    Selected task details
    Markdown notes editor/preview
    Metadata
```

### Today page

Sections:

```text
Planned for today
Done today
```

### Active page

Sections/filters:

```text
All active
Important
Due soon
Recently completed, crossed out, inside grace period
```

### Done page

Group by date:

```text
Today
Yesterday
This week
Earlier
```

### Details panel

Fields:

```text
Title
Project
Important star
Planned date
Due date
Reminder date, stored but no OS notification yet
Markdown notes editor
Markdown preview
Created/updated/completed metadata
Archive button
Reopen button if done
```

---

## 23. Keyboard shortcuts

Implement after core UI works.

Suggested shortcuts:

| Shortcut | Action |
|---|---|
| Ctrl+N | New task |
| Ctrl+F | Search |
| Ctrl+Enter | Mark selected task done |
| Ctrl+Shift+Enter | Reopen selected task |
| Delete | Archive selected task |
| Ctrl+1 | Today |
| Ctrl+2 | Active |
| Ctrl+3 | Done |
| Ctrl+S | Save edits if needed |
| Esc | Clear search or close details panel |

---

## 24. CLI build/test/run expectations

These must always work:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Linework.App
```

Codex should run `dotnet build` after every meaningful phase. If tests exist, run `dotnet test` too.

Do not move on while the build is broken.

---

## 25. Tests

Use xUnit.

Use SQLite in-memory for service/database tests, not EF Core InMemory, because SQLite behavior matters.

Example test setup:

```csharp
var connection = new SqliteConnection("DataSource=:memory:");
await connection.OpenAsync();

var options = new DbContextOptionsBuilder<LineworkDbContext>()
    .UseSqlite(connection)
    .Options;
```

Important test cases:

### TaskService

- Create task records `Created` event.
- Mark done sets status/timestamp and records `MarkedDone` event.
- Reopen clears/complements completion state and records `Reopened` event.
- Archive sets archived timestamp and records `Archived` event.
- Active query includes recently completed tasks inside grace period.
- Active query hides completed tasks outside grace period.
- Today query includes planned-for-today tasks and done-today tasks.

### ProjectService

- Create project.
- Rename project.
- Archive project.
- Optional project assignment works.

### SearchService

- Finds tasks by title.
- Finds tasks by notes.
- Finds tasks by project.
- Finds done tasks.
- Excludes archived tasks unless requested.

### SummaryService

- Returns completed tasks for a date range.
- Groups by project.
- Identifies carryover active tasks.

### MarkdownService

- Parses headings.
- Parses lists.
- Parses links.
- Rejects or ignores unsafe link schemes.

---

## 26. Implementation phases for Codex

Codex must work in small, verified phases.

### Phase 0: Documentation and skeleton plan

Create:

```text
docs/PRODUCT_SPEC.md
docs/ARCHITECTURE.md
docs/IMPLEMENTATION_PLAN.md
docs/DECISIONS.md
```

The docs should mirror this starting context but be organized for the repo.

Acceptance criteria:

- Docs exist.
- Implementation plan has phases.
- No app code yet, or only generated skeleton.

### Phase 1: WPF shell

Create:

- Solution.
- WPF project.
- Test project.
- Package references.
- Main shell window.
- Left navigation.
- Placeholder views.
- Theme setting placeholder.

Acceptance criteria:

```powershell
dotnet build
```

passes.

### Phase 2: Database and models

Create:

- Models.
- DbContext.
- EF converters.
- Initial migration.
- DbInitializer.
- AppPaths.
- Basic tests around DB creation.

Acceptance criteria:

```powershell
dotnet build
dotnet test
```

passes.

### Phase 3: Core task service

Implement:

- Create task.
- Edit task.
- Mark done.
- Reopen.
- Archive.
- Task events.
- Today/Active/Done query methods.

Acceptance criteria:

- Service tests pass.
- Done grace-period behavior is tested.

### Phase 4: Core UI functionality

Implement:

- Quick add.
- Task list.
- Details panel.
- Edit title/notes.
- Mark done/reopen/archive buttons.
- Important star.
- Optional project selector.
- Planned-for-today date.
- Persist and reload data across app restart.

Acceptance criteria:

- App can create, edit, mark done, reopen, and archive tasks.
- Data persists in SQLite.
- Done tasks are crossed out during grace period.

### Phase 5: Markdown support

Implement:

- Markdown notes editor.
- Basic preview.
- Link detection.
- Link opening in default browser.

Acceptance criteria:

- Basic markdown preview works.
- Web links open externally.
- No WebView2.

### Phase 6: Search

Implement:

- FTS5 table/migration or setup SQL.
- SearchIndexService.
- SearchService.
- Search page.
- Fallback `LIKE` search if FTS fails.

Acceptance criteria:

- Search finds title, notes, project, done tasks.
- Tests pass.

### Phase 7: Polish

Implement:

- Empty states.
- Keyboard shortcuts.
- Better grouping in Done.
- System theme support.
- Markdown export of completed tasks.
- App icon/name cleanup if desired.

Acceptance criteria:

- App feels usable as a daily tool.
- Build/test passes.

### Phase 8: Future summaries and AI

Do later only after user asks.

Implement:

- Deterministic weekly/monthly summary.
- Export summary as Markdown.
- Optional OpenAI-compatible provider.
- Optional Ollama provider.
- User-controlled AI generation.

---

## 27. First Codex prompt

Use this prompt to start the repo.

```text
You are building a Windows-native personal task and done-history app called Linework.

Before coding, read docs/CODEX_STARTING_CONTEXT.md if it exists. Treat it as canonical product and architecture context.

Goal:
Build a simple local-first WPF desktop app for Windows. It should help me track active work, todayâ€™s tasks, and completed work over time. It is not just a todo list. It is also a lightweight work ledger so future weekly/monthly summaries are possible.

Hard constraints:
- No Electron.
- No Chromium app shell.
- Use C# and .NET 10.
- Use WPF.
- Use WPF UI for modern Fluent-ish Windows styling.
- Use CommunityToolkit.Mvvm.
- Use SQLite with EF Core.
- Must build and run from the command line with dotnet build and dotnet run.
- Keep the architecture simple. Do not over-engineer.
- Make small, verifiable changes.
- After each phase, run dotnet build and fix errors before continuing.

Product requirements for v1:
- Today view.
- Active tasks view.
- Done view.
- Add, edit, mark done, reopen, and archive tasks.
- Delete should mean archive, not hard delete.
- Tasks have title, markdown notes, status, important flag, optional project, optional planned-for date, optional due date, optional reminder date, created/updated/completed/archive timestamps, and sort order.
- When a task is marked done, keep it visible and crossed out in Today/Active for a configurable grace period. Default is 1 day. After that it should hide from active views but remain in Done.
- Optional projects.
- Important/star flag.
- Markdown notes with a simple preview. Do not use WebView2 for the preview. Render a useful subset into WPF UI/FlowDocument.
- Web links inside notes should open in the default browser.
- Search should eventually search active and done tasks, titles, notes, and projects. Design for SQLite FTS5.
- System light/dark theme support.
- No AI summaries, recurring tasks, sync, tray icon, or OS notifications in the first implementation, but leave clean service boundaries so they can be added later.

Architecture:
- Use MVVM.
- Keep the app as one WPF project plus one test project.
- Create docs/PRODUCT_SPEC.md, docs/ARCHITECTURE.md, docs/IMPLEMENTATION_PLAN.md, and docs/DECISIONS.md before implementing features.
- Create services for TaskService, ProjectService, SearchService, SearchIndexService, MarkdownService, SummaryService, ExportService, ReminderService, and SettingsService.
- Add a TaskEvent table so completed-work history and future summaries are reliable.
- Store DateTimeOffset values in SQLite as UTC Unix milliseconds via EF converters.

Implementation instruction:
Start by creating the solution, WPF project, package references, test project, folder structure, placeholder shell UI, and documentation. Do not implement every feature at once. After the skeleton builds, stop and summarize what was created, how to run it, and what the next phase should be.
```

---

## 28. Codex operating rules

Codex should follow these rules while implementing:

1. Keep changes small.
2. Do not implement unrelated future features.
3. Do not silently change product direction.
4. Do not introduce Electron, WebView2, or browser-based UI unless explicitly approved later.
5. Do not add sync or accounts.
6. Do not hard-delete user data by default.
7. Always run `dotnet build` after each phase.
8. Run `dotnet test` whenever tests exist.
9. Keep ViewModels thin.
10. Put business behavior in services.
11. Use `IClock` for testable time behavior.
12. Prefer clear, boring code over clever abstractions.
13. Leave TODOs only when attached to a specific future phase.
14. Update docs if the architecture changes.
15. Keep the app CLI-buildable.

---

## 29. Design defaults

Use these unless the user changes them:

```text
App name: Linework
Default view: Today
Theme: Follow system
Done grace period: 1 day
Delete behavior: Archive
Priority model: Important star only
Project assignment: Optional
Notes format: Markdown
Markdown preview: WPF-native preview, no browser
Search: Search all non-archived tasks by default, include done tasks
Data location: %LOCALAPPDATA%\Linework\linework.db
Export format: Markdown first, JSON backup later
AI: Disabled by default, future feature only
```

---

## 30. Open questions, not blockers

These can be decided later. Do not block the first implementation on them.

- Final app name.
- Exact visual theme/colors.
- Whether Done grace period should default to 1 day or 1 week.
- Whether due/reminder fields should be visible in the first UI or only stored.
- Whether markdown preview should be live or toggle-based.
- Whether end-of-day prompt should be in-app only or Windows notification later.
- Whether AI summaries use OpenAI-compatible chat completions, Responses, Ollama native, or multiple providers.
- Whether to add global quick-add hotkey later.
- Whether to add tray icon later.

---

## 31. Final v1 acceptance criteria

The first truly usable version is complete when:

- The app launches on Windows.
- The app builds from CLI.
- The app has Today, Active, Done, Projects, Search, and Settings navigation.
- The user can create a task quickly.
- The user can edit title and markdown notes.
- The user can assign an optional project.
- The user can star important tasks.
- The user can mark tasks done.
- Done tasks remain visible and crossed out for the grace period.
- Done tasks are visible in Done history forever unless archived.
- The user can reopen completed tasks.
- The user can archive tasks.
- Data persists after closing/reopening the app.
- Basic markdown preview works.
- HTTP/HTTPS links open in the browser.
- Search works across titles, notes, projects, and done tasks.
- System theme support works or at least does not fight system dark/light mode.
- `dotnet build` passes.
- `dotnet test` passes.
- The codebase is small enough that Codex can continue iterating safely.

---

## 32. Reference links for future verification

These are not required reading for every task, but they explain the chosen stack.

- WPF overview: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/
- .NET support policy: https://dotnet.microsoft.com/en-us/platform/support/policy
- CommunityToolkit.Mvvm source generators: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/overview
- EF Core SQLite provider: https://learn.microsoft.com/en-us/ef/core/providers/sqlite/
- SQLite FTS5: https://www.sqlite.org/fts5.html
- Markdig: https://github.com/xoofx/markdig
- WPF UI: https://github.com/lepoco/wpfui
- Ollama OpenAI compatibility: https://docs.ollama.com/api/openai-compatibility


