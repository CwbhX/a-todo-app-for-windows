---
name: linework-smoke-test
description: Create focused smoke-test instructions for the Linework WPF app. Use when the user asks for a Linework smoke test, smokescreen test, manual app test script, QA checklist, or wants to report app behavior as good/not good/details during local testing.
---

# Linework Smoke Test

## Overview

Give the user a manual smoke-test script for the current Linework development stage. Always include the command to run the app, group checks into major test areas with numbered subtests, and invite the user to answer each area with `Good`, `Not good`, or details.

## Default Behavior

Default to a targeted smoke test for the change that was just made. Do not repeat the entire smoke matrix unless the user asks for a full smoke/regression pass or the change touched broad app startup, navigation, persistence, or shared task-list behavior.

When producing a targeted smoke test:

- Infer the changed area from the latest code change, user prompt, or dev diary.
- Include 1 to 3 focused test areas that directly exercise the change.
- Add a tiny launch/navigation check only if the change affects app startup, shell layout, navigation, or shared UI.
- Keep unchanged older flows out of the checklist unless they are needed setup for the changed behavior.
- Keep known gaps out of the checklist unless they are directly relevant or the user asks.

Use the full matrix below as a menu of reusable checks, not as the default response.

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

## Current Smoke Check Menu

Use these checks selectively for targeted smokes, or all together for a full smoke/regression pass.

### Launch And Navigation

1. Start the app with the required command.
2. Confirm the Linework window opens.
3. Confirm the default view is Today.
4. Click Today, Active, Done, Projects, Search, and Settings.
5. Confirm navigation does not crash.

Expected result: no crashes; Today, Active, and Done show task-list surfaces; other views may still be placeholders.

### Today Quick Add

1. In Today, add a task with a unique title.
2. Confirm it appears in Today.
3. Switch to Active.
4. Confirm the same task appears in Active.
5. Restart the app.
6. Confirm the task is still present.

Expected result: Today quick-add creates a persisted task planned for today.

### Active Quick Add And Plan Today

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

### Mark Done

1. Mark a visible active task done.
2. Confirm the row becomes dimmer and struck through.
3. Switch to Done.
4. Confirm the task appears in Done.
5. Restart the app.
6. Confirm the task remains in Done.

Expected result: completion is persisted, and recent completions remain visible wherever the service query includes them.

### Task Selection And Details

1. Select a visible task row in Today, Active, or Done.
2. Confirm the right-side Task Details panel changes from `No task selected` to the selected task title.
3. Confirm the details panel shows status, planned date, due date, completed date, and project.
4. Confirm the title and notes fields are editable.
5. Switch views and select another row.

Expected result: row selection updates the details panel without crashing and loads the selected task into the editable fields.

### Task Details Edit

1. Select a visible task row in Today or Active.
2. Change the title in the Task Details panel.
3. Add or update the notes text.
4. Click Save.
5. Confirm the task row title updates.
6. Select a different row and come back, or switch views and return.
7. Confirm the saved title and notes are still present.

Expected result: saving persists title and notes changes, and the refreshed details panel stays in sync with the selected row.

### Known Current Gaps

Mention these only when relevant:

- no edit task UI
- no reopen/archive UI
- no full markdown preview
- no search UI behavior
- no production polish

## Response Format

For targeted smokes, present only the selected checks:

```text
Command:
- ...

Targeted Smoke Test:
1. Changed Area Name
   1. ...
   Expected: ...

Report Back:
- 1 Changed Area Name: Good / Not good / Details
```

For full smokes, use the same format but title the section `Full Smoke Test` and include all applicable checks from the menu.

When the user reports results, classify each major area as pass, expected limitation, or bug candidate. If something is a bug candidate, propose the smallest next implementation or diagnostic step.
