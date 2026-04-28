# Linework - Codex Starting Context

Linework is a Windows-native, local-first personal task and done-history app. It is a hybrid between a todo list, a daily planner, and a done-history ledger.

The first version should stay simple, fast, clean, native-feeling, and easy for agents to modify. Long-term features like weekly/monthly summaries and optional AI summaries are planned, but are not part of Phase 1.

## Stack

- C#
- .NET 10
- WPF
- MVVM with CommunityToolkit.Mvvm
- SQLite with EF Core
- Markdig for markdown parsing
- WPF-UI when it can be verified cleanly
- xUnit for tests

## Naming

- Solution: `Linework`
- App project: `Linework.App`
- Test project: `Linework.Tests`
- Root namespace: `Linework`
- App display name: `Linework`
- Local data folder: `%LOCALAPPDATA%\Linework\`
- Database file: `linework.db`

## Non-goals For Phase 1

- AI summaries
- Sync
- Recurring tasks
- System tray
- OS notifications
- Full search
- Full markdown rendering
- Full production UI
