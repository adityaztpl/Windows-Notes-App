# Win Pet

A lightweight Windows desktop pet that grows from local computer usage.

## Principles

- No LLM
- No cloud dependency
- No account
- Local-first SQLite storage
- Privacy-first activity tracking
- Minimal UI
- Deterministic pet progression

## Initial architecture

```text
WinPet.App
  WPF / .NET 8
       |
       +-- Activity Monitor
       +-- Pet Engine
       +-- SQLite / Dapper
       +-- Desktop Pet Window
       +-- System Tray
```

## V1 roadmap

1. Pet window
2. SQLite persistence
3. Windows activity monitor
4. Application usage categorization
5. XP and basic stats
6. Idle/sleep behavior
7. Local activity history
8. Basic evolution system

## Privacy

The app should track coarse activity metadata only. It must not collect keystrokes, screenshots, passwords, browser history, or file contents.
