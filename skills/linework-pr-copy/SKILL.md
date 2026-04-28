---
name: linework-pr-copy
description: Draft GitHub-ready pull request titles and bodies for Linework repository changes. Use when the user asks for a PR title, PR body, pull request description, GitHub copy/paste text, or wants title and body separately after a Linework coding session.
---

# Linework PR Copy

## Overview

Produce concise, copyable GitHub PR text for the current Linework changes. Always provide the PR title and PR body separately so the user can paste them into GitHub without editing around commentary.

## Required Output

Use this exact outer shape:

```text
PR title:

```text
...
```

PR body:

```md
...
```
```

Do not put unrelated explanation inside either copy block.

## Title Guidance

Keep the title short and action-oriented.

Good examples:

- `Start persisted task UI slice`
- `Wire done grace period setting into tasks`
- `Add initial task details editing`
- `Harden database initialization tests`

Avoid vague titles like:

- `Updates`
- `Fix stuff`
- `More Linework work`

## Body Template

Use this Markdown structure unless the user asks for a different one:

```md
## Summary
- ...
- ...

## Verification
- `dotnet restore`
- `dotnet build`
- `dotnet test`
- ...

## Notes
- ...
```

If manual testing happened, include it under `Verification` as `Manual smoke pass:` with short bullets.

If the work includes intentional deferrals, list them under `Notes`.

## Content Rules

- Mention only changes actually made.
- Include docs/dev diary updates if they happened.
- Include project-local skills if they are part of the PR.
- Include failed/skipped verification only if true.
- Do not claim the app was manually tested unless the user or tools confirmed it.
- Keep the body compact enough to fit comfortably in GitHub's PR form.
- Use `Linework` capitalization.
- Do not mention AI, sync, recurring tasks, tray, notifications, FTS/search, or markdown preview unless relevant to the change or explicitly out of scope.

## When User Reports Results

If the user gives manual smoke-test results before asking for PR copy, fold the important outcomes into the `Verification` section. Classify expected limitations as notes, not failures.
