# Implementation Plan

## Phase 1: Foundation

- Create solution and projects.
- Add package references.
- Add WPF shell placeholder.
- Add MVVM ViewModels.
- Add models and DbContext.
- Add service interfaces and minimal implementations.
- Add basic TaskService tests.

## Phase 2: Database Hardening

- Keep `EnsureCreated` temporarily and document the migration strategy.
- Verify date/time converters with additional tests.
- Add settings defaults.
- Add project service tests.
- Add database creation/default-seeding tests.
- Add Done-query filtering tests.

Status: mostly complete for the current foundation. The app still uses `EnsureCreated`, with initial migrations deferred until the schema is less fluid and before builds with real user data are distributed. The seeded done grace period is now consumed by `TaskService`.

## Phase 3: Core Task Flow

- Implement quick add. Started for Today/Active.
- Show persisted task lists. Started for Today/Active/Done.
- Edit title, notes, important flag, project, planned date, due date, and reminder date.
- Mark done, reopen, and archive from the UI. Mark done is started from the minimal task list.

## Phase 4: Markdown

- Add WPF-native markdown preview using Markdig parse output.
- Open safe HTTP/HTTPS links in the default browser.

## Phase 5: Search

- Add SQLite FTS5 setup.
- Keep fallback `LIKE` search.
- Add search UI and tests.

## Phase 6: Polish

- Improve empty states.
- Add keyboard shortcuts.
- Improve Done grouping.
- Add theme polish.
- Add Markdown export.

## Future

- Deterministic weekly/monthly summaries.
- Optional AI providers behind explicit user action.
- In-app review prompts before any OS notification work.
