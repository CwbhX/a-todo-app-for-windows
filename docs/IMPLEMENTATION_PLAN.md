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

- Add migrations or a clear migration strategy.
- Verify date/time converters with additional tests.
- Add settings defaults.
- Add project service tests.

## Phase 3: Core Task Flow

- Implement quick add.
- Show persisted task lists.
- Edit title, notes, important flag, project, planned date, due date, and reminder date.
- Mark done, reopen, and archive from the UI.

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
