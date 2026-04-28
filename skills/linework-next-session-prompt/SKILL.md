---
name: linework-next-session-prompt
description: Draft high-signal handoff prompts for future Linework coding sessions or agents. Use when the user asks for a next-session prompt, handoff prompt, continuation prompt, next agent prompt, or wants to preserve current progress and next steps for another session.
---

# Linework Next Session Prompt

## Overview

Create a copyable prompt that lets the next Codex session continue Linework without rediscovering the project state. The prompt should be specific enough to prevent scope creep and broad enough to let the next agent verify the current repo before coding.

## Required Shape

Return one fenced `text` block unless the user asks for prose too.

The prompt should include these sections in this order:

1. Context to read
2. Project constraints
3. Current state
4. Project-local skills, if relevant
5. First verification steps
6. Next goal
7. Do list
8. Consider only if still small
9. Do not implement yet
10. After changes verification
11. Required final response format

## Template

Use this as the starting structure:

```text
We are continuing the Linework repo.

Before coding, read:
- AGENTS.md
- CODEX_STARTING_CONTEXT.md
- dev-docs/DEV_DIARY.md
- docs/PRODUCT_SPEC.md
- docs/ARCHITECTURE.md
- docs/IMPLEMENTATION_PLAN.md
- docs/DECISIONS.md

Linework is a Windows-native, local-first C#/.NET 10 WPF app. Keep the architecture simple: one WPF app project plus one test project, MVVM with CommunityToolkit.Mvvm, SQLite with EF Core, Markdig, no Electron, no Chromium shell, no WebView2.

Current state:
- ...

Project-local skills:
- ...

First:
1. Run `dotnet restore`
2. Run `dotnet build`
3. Run `dotnet test`
4. Search stale names:
   `WorkDone`, `workdone`, `WORKDONE`, `Work Done`, `LineWork`, `Line Work`

If build/test fails, fix the smallest issue needed to get green before adding behavior.

Next goal:
...

Do:
- ...

Consider next only if the above is green and still small:
- ...

Do not implement yet:
- AI summaries
- sync
- recurring tasks
- system tray
- OS notifications
- full search/FTS
- full markdown preview
- production visual polish
- broad UI rewrite

After changes:
- Run `dotnet restore`
- Run `dotnet build`
- Run `dotnet test`
- Run stale-name search again
- Update `dev-docs/DEV_DIARY.md` with what changed, why, verification results, known risks, and the next recommended prompt.

Finish with:
Changed:
- ...

Verified:
- ...

Notes:
- ...

Next:
- ...
```

## Content Rules

- Base the prompt on the latest diary, docs, and user reports in the current conversation.
- Mention manual smoke-test results only if the user actually reported them.
- Include known limitations as expected gaps, not failures.
- Keep the next goal narrow and directly connected to the current state.
- Preserve hard constraints: WPF, .NET 10, local-first, no Electron, no WebView2, no Chromium shell.
- Tell the next agent to fix build/test failures before adding behavior.
- Include the stale-name search every time unless the user explicitly says to skip it.
- Include project-local skills when useful:
  - `skills/linework-smoke-test` for manual smoke-test steps
  - `skills/linework-pr-copy` for GitHub PR title/body copy
  - `skills/linework-next-session-prompt` for future handoff prompts
- Avoid turning the prompt into a giant changelog. Summarize only what the next agent needs.

## Next Goal Guidance

For Linework's current early Phase 3 state, prefer next goals like:

- add a small `Plan today` action for unplanned Active tasks
- clarify row metadata such as `Unplanned` versus `Planned today`
- add basic task selection/details display
- add simple title/notes editing
- add reopen/archive UI only after the smaller flow is green

Keep AI, sync, recurring tasks, notifications, full search, markdown preview, and polish out of scope until explicitly requested.
