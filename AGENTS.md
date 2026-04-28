# AGENTS.md

Project-specific instructions for Codex and other coding agents working in this repository.

This file should stay practical and current. Prefer updating it when the agent repeats a mistake, when setup commands change, or when project conventions become clearer.

---

## Project identity

This repository contains **WorkDone**, a Windows-native, local-first personal task and done-history app.

The app is a hybrid between:

- a simple todo list,
- a daily work planner,
- and a lightweight work ledger for completed work over time.

The first version should be simple, fast, clean, and native-feeling. Long-term features like weekly/monthly summaries and optional AI-generated summaries are planned, but the first implementation should not overbuild them.

---

## Source-of-truth documents

Before making architectural or product-level decisions, read these files if present:

1. `CODEX_STARTING_CONTEXT.md`
2. `docs/PRODUCT_SPEC.md`
3. `docs/ARCHITECTURE.md`
4. `docs/IMPLEMENTATION_PLAN.md`
5. `README.md`

Use this file for recurring engineering rules. Use the docs above for deeper product and architecture context.

If this file conflicts with a more specific user prompt, follow the user prompt and then suggest updating `AGENTS.md` if the change should persist.

---

## Hard constraints

Do not violate these unless the user explicitly changes direction.

- Windows native desktop app.
- No Electron.
- No Chromium app shell.
- Do not use WebView2 for the app shell or Markdown preview.
- C#.
- .NET 10.
- WPF.
- WPF-UI for modern Fluent-ish Windows styling when practical.
- CommunityToolkit.Mvvm for MVVM.
- SQLite with EF Core for local persistence.
- Markdig for Markdown parsing.
- Local-first by default.
- Visual Studio may be installed, but normal development should work from CLI.
- Keep the architecture simple and easy for agents to modify.
- Do not add sync, recurring tasks, system tray, OS notifications, or AI calls in the first version unless explicitly requested.

---

## Development style

Work in small, verifiable steps.

Prefer this loop:

1. Inspect relevant files.
2. Make a focused change.
3. Build.
4. Test.
5. Fix errors.
6. Summarize what changed and what remains.

Do not do giant rewrites unless the user explicitly asks for a rewrite.

Avoid broad “cleanup” edits unrelated to the task. If cleanup is valuable, mention it as a follow-up.

Do not introduce clever abstractions before the app needs them. This repo should stay friendly to quick iteration.

---

## CLI commands

Use these commands from the repository root.

Restore:

```powershell
dotnet restore
```

Build on Windows:

```powershell
dotnet build
```

Build from non-Windows environments, if needed:

```bash
dotnet build -p:EnableWindowsTargeting=true
```

Run app on Windows:

```powershell
dotnet run --project src/WorkDone.App
```

Run tests:

```powershell
dotnet test
```

If formatting is configured:

```powershell
dotnet format
```

Do not claim a change is done unless the relevant build/test command has been run, or unless you clearly state why it could not be run.

---

## Expected repository layout

Keep the repo close to this shape:

```text
src/
  WorkDone.App/
    Views/
    ViewModels/
    Models/
    Data/
    Services/
      Ai/
    Infrastructure/

tests/
  WorkDone.Tests/

docs/
  PRODUCT_SPEC.md
  ARCHITECTURE.md
  IMPLEMENTATION_PLAN.md
```

If you need to add a new directory, keep it obvious and document it if it affects architecture.

---

## Architecture rules

Use a simple WPF MVVM architecture.

Recommended layering:

```text
Views -> ViewModels -> Services -> Data/Models
```

Rules:

- Views should contain XAML and minimal code-behind.
- ViewModels should use CommunityToolkit.Mvvm.
- ViewModels should not directly own EF Core query logic.
- Services should contain application behavior.
- `WorkDoneDbContext` should be the EF Core boundary.
- Keep external integrations behind interfaces.
- Do not create a separate class library unless there is a strong reason.
- Do not introduce MediatR, CQRS, Redux-style state management, or plugin systems for v1.
- Do not create “enterprise clean architecture” folders just to look formal.

---

## Domain model expectations

Core entities:

- `TaskItem`
- `TaskEvent`
- `Project`
- `AppSetting`

Core task behavior:

- Create task.
- Edit task.
- Mark done.
- Reopen.
- Archive.
- Query Today, Active, Done, and Search.

Important product rule:

> Delete means archive by default. Do not hard-delete user tasks in normal app behavior.

Task completion should preserve history:

- Set status to `Done`.
- Set `CompletedAt`.
- Add a `TaskEvent`.
- Keep recently completed tasks visible and crossed out for the configured grace period.
- Continue showing all completed tasks in the Done view.

Use `TaskEvent` for history because future weekly/monthly summaries depend on accurate event data.

---

## Data and persistence

Use SQLite through EF Core.

Default database path should be under:

```text
%LOCALAPPDATA%\WorkDone\workdone.db
```

Do not store user data in the repo.

Do not commit generated local databases, logs, user settings, or secrets.

For tests, prefer SQLite in-memory or an isolated temporary database.

If using migrations:

- Keep migrations readable.
- Do not hand-edit migrations unless necessary.
- Confirm migrations build and apply cleanly.

---

## Markdown rules

Markdown is a first-class direction for task notes, but keep v1 simple.

Use Markdig for parsing.

Do not use WebView2 for Markdown preview.

A useful v1 preview can support:

- headings,
- paragraphs,
- bold/italic,
- inline code,
- fenced code blocks,
- bullets,
- numbered lists,
- links,
- checkboxes if easy.

Links in notes should open in the default browser.

Do not build a full rich-text editor in v1.

---

## Search rules

Search should eventually feel “smart,” but keep early implementation simple.

v1 search should aim to cover:

- task title,
- Markdown notes,
- project name,
- active tasks,
- done tasks,
- archived tasks only if explicitly included.

SQLite FTS5 is the preferred long-term direction.

Do not add a heavy external search service.

---

## AI feature rules

AI summaries are a future feature, not v1 core behavior.

Design for:

- OpenAI-compatible APIs,
- Ollama/local models,
- provider abstraction via `IAiSummaryProvider`.

Do not make external API calls unless the user explicitly asks for AI implementation.

Do not hardcode API keys.

Do not log secrets.

AI should work from structured local summary data. The app’s deterministic summary logic should not depend on an LLM.

---

## Reminder and notification rules

The user wants an eventual end-of-day prompt, but not a first-pass notification system.

For v1:

- It is okay to store `DueAt` and `ReminderAt`.
- It is okay to design interfaces for reminders.
- Do not implement Windows OS notifications unless explicitly requested.
- Do not add recurring tasks yet.

---

## UI/UX rules

The app should feel like a clean modern Windows utility.

Default mental model:

- Today is the home view.
- Active shows unfinished work.
- Done shows historical completed work.
- Projects are optional grouping.
- Search can find both current and completed work.

Avoid clutter.

Prefer:

- obvious actions,
- keyboard-friendly flows,
- clean empty states,
- simple navigation,
- fast startup,
- readable task rows,
- visible completed-task satisfaction without polluting active views forever.

Task row behavior:

- Important/star should be quick to toggle.
- Mark done should be quick.
- Recently done tasks may remain crossed out for a grace period.
- Archived tasks should disappear from normal views.

Theme behavior:

- Follow system theme by default.
- Do not overbuild theme customization.

---

## Testing expectations

Add or update tests when changing behavior in:

- task lifecycle,
- database persistence,
- event logging,
- search,
- markdown parsing,
- summary generation,
- export.

Minimum useful service tests:

- Create task.
- Mark done.
- Reopen.
- Archive.
- Event records are written.
- Done queries include completed tasks.
- Active queries exclude archived tasks.

Prefer tests around services and data behavior over fragile WPF UI automation in early versions.

If a test cannot run in the current environment, state why.

---

## Dependency rules

Before adding a new production dependency, consider whether it is necessary.

Allowed/expected dependencies include:

- CommunityToolkit.Mvvm
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Hosting
- WPF-UI
- Markdig
- xUnit test packages

Avoid adding:

- Electron,
- WebView2,
- Chromium wrappers,
- heavy UI frameworks,
- broad architecture frameworks,
- cloud sync SDKs,
- telemetry SDKs,
- AI SDKs before the AI phase.

If a package is added, explain why in the final summary.

---

## Git and file hygiene

Do not commit or generate unnecessary binary files.

Do not commit:

- `bin/`
- `obj/`
- `.vs/`
- local SQLite databases,
- logs,
- secrets,
- API keys,
- user-specific settings.

Keep generated files out of source control unless they are intentionally part of the project.

Do not run destructive git commands unless the user explicitly requests them.

Avoid commands such as:

```bash
git reset --hard
git clean -fd
git checkout -- .
```

unless the user clearly asks for that behavior.

---

## Documentation expectations

Update docs when behavior or architecture changes.

Use:

- `README.md` for setup and current project status.
- `docs/PRODUCT_SPEC.md` for product requirements.
- `docs/ARCHITECTURE.md` for technical decisions.
- `docs/IMPLEMENTATION_PLAN.md` for sequencing.

Do not let docs claim features are complete unless they actually exist.

---

## Definition of done

For a normal coding task, done means:

1. The requested behavior is implemented.
2. The app builds.
3. Relevant tests pass or limitations are clearly stated.
4. The diff is focused.
5. No unrelated files were modified.
6. No secrets, binaries, or local artifacts were committed.
7. Any docs affected by the change were updated.
8. The final response explains:
   - what changed,
   - how it was verified,
   - known limitations,
   - recommended next step.

---

## How to respond after work

End each task with a compact summary:

```text
Changed:
- ...

Verified:
- ...

Notes:
- ...

Next:
- ...
```

If something failed, include the exact command and error summary.

Do not hide failed builds or skipped tests.

---

## When blocked

Do not spin indefinitely.

If blocked:

1. State the blocker.
2. Show the relevant error.
3. Explain the likely cause.
4. Propose the smallest next action.

If a Windows-only command cannot run in the current environment, still run what can be run, such as restore, non-Windows build with `EnableWindowsTargeting`, or tests that do not require WPF runtime.

---

## Current product defaults

Unless changed by the user, assume:

- App name: `WorkDone`.
- Default view: Today.
- Done grace period: 1 day.
- Delete/archive behavior: archive by default.
- Priority model: important star plus planned/due dates.
- Projects: optional.
- Subtasks: simple Markdown checkboxes in notes, not real subtask entities.
- Time tracking: skipped.
- Recurring tasks: skipped.
- Sync: skipped.
- AI summaries: future phase.
- OS notifications: future phase.
- Tray icon/global hotkey: future phase.
