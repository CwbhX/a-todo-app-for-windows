---
name: linework-smoke-test
description: Create and maintain structured smoke-test instructions for the Linework WPF app. Use when the user asks for a Linework smoke test, smokescreen test, manual app test script, QA checklist, or wants to report app behavior as good/not good/details during local testing.
---

# Linework Smoke Test

## Overview

Give the user a manual smoke-test script for the current Linework development stage. Always include the command to run the app, group checks into major test areas with numbered subtests, and invite the user to answer each area with `Good`, `Not good`, or details.

## Required Command

Always start with this command from the repository root:

```powershell
dotnet run --project src/Linework.App
```

If the user is testing after code changes and asks for verification too, include:

```powershell
dotnet restore
dotnet build
dotnet test
```

## Current Smoke Matrix

Use these major test areas unless the repo has clearly moved beyond them.

### 1. Launch And Navigation

1. Start the app with the required command.
2. Confirm the Linework window opens.
3. Confirm the default view is Today.
4. Click Today, Active, Done, Projects, Search, and Settings.
5. Confirm navigation does not crash.

Expected result: no crashes; Today, Active, and Done show task-list surfaces; other views may still be placeholders.

### 2. Today Quick Add

1. In Today, add a task with a unique title.
2. Confirm it appears in Today.
3. Switch to Active.
4. Confirm the same task appears in Active.
5. Restart the app.
6. Confirm the task is still present.

Expected result: Today quick-add creates a persisted task planned for today.

### 3. Active Quick Add And Plan Today

1. In Active, add a task with a unique title.
2. Confirm it appears in Active.
3. Switch to Today.
4. Confirm it does not appear in Today.
5. Switch back to Active and choose Plan today for that task.
6. Confirm its row metadata changes from Unplanned to Planned today.
7. Switch to Today.
8. Confirm the task now appears in Today.
9. Restart the app and check Today again.

Expected result: Active quick-add creates a persisted unplanned active task, and Plan today moves that task into Today.

### 4. Mark Done

1. Mark a visible active task done.
2. Confirm the row becomes dimmer and struck through.
3. Switch to Done.
4. Confirm the task appears in Done.
5. Restart the app.
6. Confirm the task remains in Done.

Expected result: completion is persisted, and recent completions remain visible wherever the service query includes them.

### 5. Known Current Gaps

Ask the user to note these as expected gaps, not failures:

- no edit task UI
- no reopen/archive UI
- no task details behavior
- no search UI behavior
- no markdown preview
- no production polish

## Response Format

Present the checklist in this format:

```text
Command:
- ...

Smoke Test:
1. Launch And Navigation
   1. ...
   Expected: ...

2. Today Quick Add
   1. ...
   Expected: ...

Report Back:
- 1 Launch And Navigation: Good / Not good / Details
- 2 Today Quick Add: Good / Not good / Details
- 3 Active Quick Add And Plan Today: Good / Not good / Details
- 4 Mark Done: Good / Not good / Details
- 5 Known Gaps: Anything surprising?
```

When the user reports results, classify each major area as pass, expected limitation, or bug candidate. If something is a bug candidate, propose the smallest next implementation or diagnostic step.
