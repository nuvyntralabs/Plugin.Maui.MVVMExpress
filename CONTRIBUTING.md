# Contributing to MVVMExpress

Thank you for helping. This repository is an incremental MVVM framework. Please read [ARCHITECTURE.md](ARCHITECTURE.md) and [DESIGN-PLAN.md](DESIGN-PLAN.md) before opening a large PR.

## Rules

1. Do not implement a later roadmap phase in the same PR as an earlier one unless the earlier phase is blocked.
2. Core (`Plugin.Maui.MVVMExpress.Core`) must not reference `Microsoft.Maui.Controls`, System.Reactive, CommunityToolkit.Mvvm, Prism, or FluentValidation. New Core APIs need tests listed in [docs/TEST-COVERAGE.md](docs/TEST-COVERAGE.md).
3. Do not copy type names from CommunityToolkit.Mvvm or Prism (`ObservableObject`, `RelayCommand`, `INavigationService`, `IMessenger`).
4. Do not add a PackageReference to a sibling MauiEssentials plugin. Compose through adapters in the app or a documented sample.
5. Public APIs need XML documentation.
6. Every catch must transform to `Outcome`, call `IErrorSink`, log and rethrow, or map `OperationCanceledException` to Cancelled. Empty `catch { }` is rejected.
7. Library async code uses `ConfigureAwait(false)` except where the next step needs the UI context.
8. Do not commit secrets. Do not log tokens or passwords.
9. Tests for Core run on `net10.0` without MAUI.

## Workflow

```bash
dotnet build Plugin.Maui.MVVMExpress.slnx
dotnet test Plugin.Maui.MVVMExpress.slnx
```

Use `dotnet format` before you push. Warnings are errors on packable projects.

## Pull requests

Use the PR template. Describe the phase, the public API change (if any), and the tests added. Memory-leak changes need a `WeakReference` test.
