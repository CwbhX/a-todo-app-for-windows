# Decisions

## 2026-04-27: Rename To Linework

The app is named Linework. Use `Linework`, `Linework.App`, `Linework.Tests`, root namespace `Linework`, `%LOCALAPPDATA%\Linework\`, and `linework.db`.

## 2026-04-27: Keep Phase 1 Small

Phase 1 sets up the foundation only. Core UI workflows, search, markdown rendering, summaries, AI, sync, recurring tasks, tray behavior, and OS notifications stay out of scope.

## 2026-04-27: Plain WPF Shell First

The project references WPF-UI for future styling, but the placeholder shell uses plain WPF controls until package restore/build can be verified on a machine with the .NET 10 SDK available.

## 2026-04-28: Keep EnsureCreated Temporarily

Linework will keep `EnsureCreated` during the early foundation/database-hardening phase. This avoids adding EF migration tooling before the schema has settled. Initial migrations should be added before distributing builds that may contain real user data.

## 2026-04-28: Seed First-Run Settings Defaults

Database initialization seeds system theme mode, Today as the default view, and a one-day done grace period if those settings do not already exist.

## 2026-04-28: Consume Done Grace Period Through Settings

`TaskService` reads `tasks.doneGracePeriodDays` when querying Active tasks. If the setting is absent, it falls back to one day so service tests and older databases keep the product default.

## 2026-04-28: Start UI With A Minimal Persisted Task Loop

The first real UI slice stays inside the existing WPF shell: quick-add, persisted Today/Active/Done rows, and mark-done. Full editing, details behavior, archive/reopen UI, markdown preview, search, and polish remain deferred.

## 2026-04-28: Add Minimal Plan Today Flow

Unplanned Active tasks can now be planned for today from the task row. This keeps Today simple: it still only queries planned-for-today, due-today, or completed-today tasks, while the service updates the existing task's `PlannedForDate` and records a `PlannedForDateChanged` event.
