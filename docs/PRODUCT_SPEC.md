# Product Spec

Linework is a local-first Windows utility for tracking active work, today's plan, and completed work over time.

## Core Model

- Today is the home view.
- Active shows unfinished work plus recently completed work during the done grace period.
- Done preserves completion history.
- Projects are optional.
- Delete means archive by default.

## Phase 1 Scope

- WPF shell with left navigation, main content placeholder, and task details placeholder.
- Initial domain model for tasks, projects, events, and settings.
- EF Core SQLite persistence boundary.
- Basic TaskService lifecycle behavior and tests.

## Deferred

- Full CRUD UI
- Full markdown preview
- Search and FTS5
- Export
- Weekly/monthly summaries
- Optional AI providers
- Windows notifications
- Recurring tasks
- Sync
