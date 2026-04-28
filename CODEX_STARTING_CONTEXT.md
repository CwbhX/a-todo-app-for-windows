# Linework - Codex Starting Context

Linework is a Windows-native, local-first personal task and done-history app. It helps answer:

```text
What do I need to do?
What am I trying to get done today?
What did I finish?
What did I accomplish this week or month?
```

Use C#, .NET 10, WPF, MVVM, CommunityToolkit.Mvvm, SQLite with EF Core, and Markdig. Do not use Electron, Chromium app shells, or WebView2.

Default names:

- Solution: `Linework`
- App project: `Linework.App`
- Test project: `Linework.Tests`
- Root namespace: `Linework`
- App display name: `Linework`
- Local data folder: `%LOCALAPPDATA%\Linework\`
- Database file: `linework.db`

Phase 1 is only the foundation. Do not build AI summaries, sync, recurring tasks, system tray, OS notifications, full search, full markdown rendering, or production UI yet.

See `docs/PRODUCT_SPEC.md`, `docs/ARCHITECTURE.md`, and `docs/IMPLEMENTATION_PLAN.md` for the working repo docs.
