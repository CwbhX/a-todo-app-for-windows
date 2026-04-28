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
