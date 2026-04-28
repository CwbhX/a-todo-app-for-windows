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

## 2026-04-28 - Minimal Plan Today Action

### Starting Point

The user asked to continue Linework Phase 3 by verifying the foundation, then making the smallest useful task-flow improvement around unplanned Active tasks.

Read before coding:

- `AGENTS.md`
- `CODEX_STARTING_CONTEXT.md`
- `dev-docs/DEV_DIARY.md`
- `docs/PRODUCT_SPEC.md`
- `docs/ARCHITECTURE.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/DECISIONS.md`

Baseline verification before edits:

```powershell
dotnet restore
dotnet build
dotnet test
```

Results:

- restore succeeded
- build succeeded with 0 warnings and 0 errors
- tests passed: 15 passed, 0 failed, 0 skipped

The legacy-name sweep initially found one self-referential match in `skills/linework-next-session-prompt/SKILL.md`. That skill now refers to the repo instructions instead of embedding the old literal terms.

### Changes

Added `TaskService.PlanForTodayAsync`. It sets an existing task's `PlannedForDate` to `clock.Today`, updates `UpdatedAt`, and records a `PlannedForDateChanged` event.

Added a focused service test proving that planning an unplanned task for today:

- sets the planned date
- records the event
- makes the task appear in the Today query

Updated the minimal WPF row behavior:

- unplanned Active rows now show `Unplanned` instead of `Active`
- tasks planned for the current day show `Planned today`
- unplanned Active rows expose a row-level `Plan today` action
- planning refreshes the current list through the existing MVVM command flow

Updated docs and project-local skills:

- `docs/PRODUCT_SPEC.md`
- `docs/ARCHITECTURE.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/DECISIONS.md`
- `skills/linework-smoke-test/SKILL.md`
- `skills/linework-next-session-prompt/SKILL.md`

### Verification Results

Final verification after code and docs updates:

```powershell
dotnet restore
dotnet build
dotnet test
```

Results:

- restore succeeded
- build succeeded with 0 warnings and 0 errors
- tests passed: 16 passed, 0 failed, 0 skipped

The final legacy-name sweep found no matches.

### Manual Smoke Test

The project-local `skills/linework-smoke-test` skill was updated to include the new Active quick-add plus Plan today flow. Future smoke tests should verify that an Active-created task starts as Unplanned, does not appear in Today until planned, then appears in Today after using Plan today.

The user ran the updated manual smoke test after implementation and reported all tested areas as good:

- Launch and navigation worked.
- Today quick-add worked.
- Active quick-add plus Plan today worked.
- Mark done worked.

Conclusion: the minimal persisted task loop and new Plan today flow have both automated verification and a clean manual smoke pass.

### Known Risks

This is still the intentionally minimal row-based UI. There is no selection/details behavior, title/notes editing, reopen/archive UI, or richer date control yet.

`PlanForTodayAsync` is deliberately narrow. A future general planning/editing flow may want a more flexible `PlanForDateAsync` or task update method once details editing exists.

### Recommended Next Prompt

```text
Continue Linework Phase 3. First read the standard repo context docs and run dotnet restore, dotnet build, dotnet test, plus the legacy app-name sweep from AGENTS/current handoff instructions. If anything is red, fix the smallest issue first.

Next goal: add the next smallest useful task-flow improvement after Plan today. Prefer basic task selection/details display, then simple title/notes editing if still small. Keep WPF changes minimal and MVVM-shaped.

Do not add AI summaries, sync, recurring tasks, system tray, OS notifications, full search/FTS, full markdown preview, production visual polish, or a broad UI rewrite.

After changes, rerun restore/build/test and the legacy-name sweep, update dev-docs/DEV_DIARY.md with what changed and verification results, and use skills/linework-smoke-test if producing manual smoke steps.
```

---

## 2026-04-28 - Project-Local Smoke Test Skill

Added a project-local Codex skill at `skills/linework-smoke-test` after the user asked for a reusable "smokescreen" testing workflow. The skill is intentionally local to the repository, not installed into the global Codex skills directory.

The skill tells future agents to always provide the app run command:

```powershell
dotnet run --project src/Linework.App
```

It also structures the current manual smoke test into major areas with subtests:

- Launch and navigation
- Today quick add
- Active quick add
- Mark done
- Known current gaps

The expected user reporting format is `Good`, `Not good`, or details per major area, so future sessions can quickly classify results as pass, expected limitation, or bug candidate.

Updated `AGENTS.md` to point future agents at this project-local skill.

Verification:

```powershell
python 'C:\Users\Clement Hathaway\.codex\skills\.system\skill-creator\scripts\quick_validate.py' skills/linework-smoke-test
```

Result: skill is valid.

---

## 2026-04-28 - Project-Local PR Copy Skill

Added a project-local Codex skill at `skills/linework-pr-copy` after the user asked to make the reusable PR title/body workflow into a project skill.

The skill tells future agents to produce GitHub-ready PR text with separate copyable blocks:

- PR title
- PR body

The body template includes Summary, Verification, and Notes sections, and it reminds agents to include only changes and verification that actually happened.

Updated `AGENTS.md` to point future agents at this project-local skill.

Verification:

```powershell
python 'C:\Users\Clement Hathaway\.codex\skills\.system\skill-creator\scripts\quick_validate.py' skills/linework-pr-copy
```

Result: skill is valid.

---

## 2026-04-28 - Project-Local Next Session Prompt Skill

Added a project-local Codex skill at `skills/linework-next-session-prompt` after the user asked for a reusable way to create useful prompts for the next session or agent.

The skill tells future agents to generate a copyable handoff prompt with:

- context files to read
- Linework project constraints
- current state
- project-local skills
- first verification steps
- a narrow next goal
- explicit do/do-not-implement lists
- post-change verification
- required final response format

Updated `AGENTS.md` to point future agents at this project-local skill.

Verification:

```powershell
python 'C:\Users\Clement Hathaway\.codex\skills\.system\skill-creator\scripts\quick_validate.py' skills/linework-next-session-prompt
```

Result: skill is valid.

---

## 2026-04-28 - Project-Local Plugin For Skills

Initial root-level `commands/*.md` files did not appear in Codex's slash menu. A small project-local plugin was then added, but official OpenAI docs clarified that Codex app slash commands are built-ins and enabled skills may appear in the slash list. The plugin docs describe plugins as bundling skills, apps, and MCP servers; arbitrary plugin `commands/*.md` app slash commands are not documented.

Added:

- `.agents/plugins/marketplace.json`
- `plugins/linework/.codex-plugin/plugin.json`
- `plugins/linework/skills/linework-smoke-test`
- `plugins/linework/skills/linework-pr-copy`
- `plugins/linework/skills/linework-next-session-prompt`

The plugin now bundles copies of the three skills directly under `plugins/linework/skills/` and declares `"skills": "./skills/"` in its manifest.

Updated `AGENTS.md` with the plugin location and the expected install/use flow.

After restarting Codex, the custom commands still did not appear. The marketplace entry was briefly changed to `policy.installation: "INSTALLED_BY_DEFAULT"`, but after checking docs the repo returned to the documented `AVAILABLE` marketplace pattern and now relies on plugin install/enablement.

Verification:

- Confirmed plugin and marketplace JSON parse.
- Re-ran validation for the three plugin-bundled skills.

Expected use:

1. Restart Codex after the marketplace/plugin files exist.
2. Open `/plugins`.
3. Look for the repo marketplace / Linework plugin and install or enable it.
4. Use the skills from `$` or from `/` if Codex surfaces enabled skills in the slash list.

Important limitation: project-local custom slash commands themselves do not appear to be supported/documented in the Codex app at this time. Skills are the supported route.

---

## 2026-04-28 - Minimal Persisted Task UI Start

### Starting Point

The user asked to continue Linework by verifying the current foundation first, then moving into the smallest useful vertical slice toward a real app UI.

The requested starting reads were completed:

- `AGENTS.md`
- `CODEX_STARTING_CONTEXT.md`
- `dev-docs/DEV_DIARY.md`
- `docs/PRODUCT_SPEC.md`
- `docs/ARCHITECTURE.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/DECISIONS.md`

### Foundation Verification

Initial sandboxed `dotnet restore` failed because the SDK tried to create first-run files under `C:\Users\CodexSandboxOffline\.dotnet`, which was not writable from the sandbox. The same issue affected sandboxed build/test. The commands succeeded when run with approved elevated permissions.

Initial Git status also hit Git's dubious-ownership protection because the repo owner SID differs from the sandbox user SID. Git reads in this session used `git -c safe.directory='C:/Users/Clement Hathaway/GitHub/a-todo-app-for-windows' ...` instead of changing global config.

Baseline results before edits:

```powershell
dotnet restore
dotnet build
dotnet test
```

- restore succeeded
- build succeeded with 0 warnings and 0 errors
- tests passed: 14 passed, 0 failed, 0 skipped

### Changes

Wired the seeded `tasks.doneGracePeriodDays` setting into `TaskService.GetActiveTasksAsync`. The service now reads the setting through `ISettingsService` and falls back to one day if the setting is missing. Negative configured values are clamped to zero days.

Added a focused service test proving that a configured three-day grace period keeps completed tasks visible in Active for two days and hides them after four days.

Started the smallest persisted WPF task loop:

- quick-add textbox and button for Today/Active
- Today quick-add creates a task planned for today so it appears immediately in the Today query
- Active quick-add creates an unplanned active task
- Today, Active, and Done views display persisted rows from `TaskService`
- rows can be marked done from the UI
- done rows render with strikethrough text and remain visible wherever service queries include them

Kept WPF changes deliberately small. `MainWindow` only triggers initial async load in code-behind; task behavior stays in `MainViewModel` and `TaskService`.

Updated:

- `src/Linework.App/Services/TaskService.cs`
- `src/Linework.App/ViewModels/MainViewModel.cs`
- `src/Linework.App/MainWindow.xaml`
- `src/Linework.App/MainWindow.xaml.cs`
- `tests/Linework.Tests/TaskServiceTests.cs`
- `docs/PRODUCT_SPEC.md`
- `docs/ARCHITECTURE.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/DECISIONS.md`
- `dev-docs/DEV_DIARY.md`

### Verification Results

Post-change verification:

```powershell
dotnet build
dotnet test
```

- build succeeded with 0 warnings and 0 errors
- tests passed: 15 passed, 0 failed, 0 skipped

Final requested verification was run after the docs update:

```powershell
dotnet restore
dotnet build
dotnet test
```

- restore succeeded
- build succeeded with 0 warnings and 0 errors
- tests passed: 15 passed, 0 failed, 0 skipped

The requested stale-name variants were searched. The first pass only matched this diary because it listed the literal search terms, so the diary wording was changed to avoid creating a false positive. The final stale-name search found no matches.

### Manual Smoke Test Results

The user manually ran the app and checked the current vertical slice.

Observed results:

- Today quick-add worked as expected.
- Active showed the task originally created from Today.
- Active quick-add created a task that appeared in Active.
- The task created from Active did not appear in Today. This is expected for the current implementation because Active quick-add creates an unplanned task and there is not yet a "Plan today" action.
- The Active-created row only shows `Active` under the title. This is accurate but not very explanatory; clearer metadata such as `Unplanned` or a future `Plan today` action would improve the mental model.
- Marking a visible task done added a strikethrough and dimmed the row.
- The completed task appeared in Done.
- After restarting the app, the completed task was still in Done.
- Navigation produced no crashes.

Conclusion: the current minimal persisted task loop is behaving as designed. The main UX gap found by manual testing is the lack of a way to move an existing Active/unplanned task onto Today.

### Known Risks

The UI has now had a basic manual smoke pass, but not a broad interaction pass. Focus behavior, keyboard flow, error surfaces, and longer lists still need more real use.

`MainViewModel` is now doing the first task-list orchestration. That is acceptable for this small slice, but as details editing grows, it may be worth extracting row/detail ViewModels rather than bloating the main shell.

`EnsureCreated` is still in use. This remains acceptable during early foundation work but must change before real user data is at risk.

No dedicated session doc was added for this work. The dev diary is enough for this size of handoff; source-of-truth docs already capture the product and architecture changes.

### Recommended Next Prompt

```text
Continue Linework Phase 3. First run dotnet restore, dotnet build, dotnet test, and stale-name searches. Then improve the minimal task UI without broad polish: add a small "Plan today" action or task details affordance for unplanned Active tasks, add selection/details for task title and notes, support reopen/archive through the service, and keep tests focused on service behavior. Do not add AI, sync, recurring tasks, notifications, search/FTS, or markdown preview yet.
```

---

## 2026-04-28 - Phase 2 Database Hardening Start

### Starting Point

The user asked to continue from the Phase 1 foundation, first verifying the SDK/build baseline and then staying inside Phase 2 only: database hardening, settings defaults, converter tests, DB creation tests, ProjectService tests, and Done-query tests.

The .NET SDK was initially installed but not visible on PATH in this Codex shell. `C:\Program Files\dotnet\dotnet.exe` existed and reported SDK `10.0.203`. The user PATH was updated to include `C:\Program Files\dotnet`; Codex still prepended that path in commands because the running shell had already inherited the old environment.

### Foundation Verification

Ran:

```powershell
dotnet --info
dotnet restore
dotnet build
dotnet test
```

Initial build found two Phase 1 compile issues:

- `AppPaths.cs` needed `using System.IO`.
- `NullableDateOnlyIsoStringConverter` used `value is null` inside an EF expression tree, which is not supported there.

Both were fixed with minimal edits. After that, the foundation built and the original 6 tests passed.

### Phase 2 Changes

Kept `EnsureCreated` temporarily and documented the decision in `docs/DECISIONS.md`, `docs/ARCHITECTURE.md`, and `docs/IMPLEMENTATION_PLAN.md`.

Why: the schema is still early and fluid. Adding EF migrations now would require extra tooling before there is real user data to preserve. Initial migrations should be added before distributing builds that may contain real user data.

Added first-run settings defaults:

- `theme.mode` = `System`
- `navigation.defaultView` = `Today`
- `tasks.doneGracePeriodDays` = `1`

Added enums for those defaults:

- `AppThemeMode`
- `AppDefaultView`

Updated `SettingsService` to serialize enums as readable JSON strings via `JsonStringEnumConverter`.

Added/expanded tests:

- date/time converter round trips for `DateTimeOffset`, nullable `DateTimeOffset`, and nullable `DateOnly`
- file-backed DB creation plus default settings seeding via `DbInitializer`
- ProjectService create/list behavior
- TaskService Done-query behavior for completed-only results and completed-date filtering

Cleaned the existing xUnit analyzer warnings by passing `TestContext.Current.CancellationToken` through test async calls.

### Files Changed

- `.gitignore`
- `src/Linework.App/Data/Converters/NullableDateOnlyIsoStringConverter.cs`
- `src/Linework.App/Data/DbInitializer.cs`
- `src/Linework.App/Infrastructure/AppPaths.cs`
- `src/Linework.App/Models/Enums.cs`
- `src/Linework.App/Services/SettingsService.cs`
- `tests/Linework.Tests/DateTimeConverterTests.cs`
- `tests/Linework.Tests/DbInitializerTests.cs`
- `tests/Linework.Tests/ProjectServiceTests.cs`
- `tests/Linework.Tests/TaskServiceTests.cs`
- `tests/Linework.Tests/TestDbFactory.cs`
- `docs/ARCHITECTURE.md`
- `docs/IMPLEMENTATION_PLAN.md`
- `docs/DECISIONS.md`
- `dev-docs/DEV_DIARY.md`

### Verification Results

Final verification:

```powershell
dotnet restore
dotnet build
dotnet test
```

Results:

- restore succeeded
- build succeeded with 0 warnings and 0 errors
- tests passed: 14 passed, 0 failed, 0 skipped

Also searched for the requested stale-name variants. No stale-name matches remained.

### Known Risks

The app still uses `EnsureCreated`, so migrations are not yet available for schema evolution. This is acceptable for this early phase but must be revisited before real user data is at risk.

`TaskService` still has a hardcoded one-day done grace period. The default setting is seeded, but the service does not consume it yet. That can wait until settings are wired into runtime behavior.

### Recommended Next Prompt

```text
Continue Linework Phase 2/early Phase 3. First run dotnet restore, dotnet build, and dotnet test. Then wire the seeded done grace period setting into TaskService without overbuilding settings UI. Keep EnsureCreated for now unless you are ready to add the initial EF migration. Begin the smallest persisted task-list UI step only if the service/data layer remains green.
```

---

## 2026-04-27 - Phase 1 Foundation And Rename To Linework

### Starting Point

The repo was effectively blank except for agent/project context:

- `AGENTS.md`
- `dev-docs/CODEX_STARTING_CONTEXT.md`

Both documents still described the app by its previous name. The user clarified that the app is now named `Linework` and asked for Phase 1 only: solution setup, WPF app project, xUnit test project, basic repo structure, placeholder WPF shell, initial MVVM ViewModels, initial domain models, EF Core SQLite DbContext, service interfaces/placeholders, starter docs, and practical TaskService tests.

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

Why: both still used the previous app name. The user explicitly requested stale references to be updated across repo and docs.

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

Searched for the requested stale-name variants. Final case-sensitive search found no matches.

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
- Rename project/docs from the previous app name to Linework
- Add initial .NET 10 WPF solution with `Linework.App` and `Linework.Tests`
- Add placeholder shell UI, MVVM ViewModels, domain models, EF Core SQLite DbContext, infrastructure, and service boundaries
- Add basic `TaskService` lifecycle behavior and SQLite in-memory tests
- Add starter docs for product spec, architecture, implementation plan, and decisions

## Verification
- Searched for the requested stale-name variants
- `dotnet restore`, `dotnet build`, and `dotnet test` could not run because `dotnet` is not available on PATH in the current environment

## Notes
- WPF-UI is referenced but the shell currently uses plain WPF until restore/build can verify it cleanly
- This is Phase 1 foundation only; no AI, sync, recurring tasks, tray, OS notifications, full search, full markdown rendering, or production UI
```

### Recommended Next Prompt

```text
Phase 2: First, make sure the .NET 10 SDK is installed and available on PATH. Run dotnet restore, dotnet build, and dotnet test. Fix any compile/test issues from the Phase 1 foundation. Then harden the database layer: decide whether to use migrations now or keep EnsureCreated temporarily, add settings defaults, verify date/time converters with tests, and add ProjectService persistence tests. Keep the scope small and do not implement the full UI yet.
```
