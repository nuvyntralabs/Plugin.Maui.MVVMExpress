# Plugin.Maui.MVVMExpress — AI Coding Agent Guide

## Project

Modular MVVM application framework for .NET MAUI.

- Product: MVVMExpress
- Package prefix: `Plugin.Maui.MVVMExpress`
- Catalog: https://github.com/nuvyntralabs/MauiEssentials
- Status: **1.0.0.** Navigators construct pages on `IMainThread`. `UseNavigationPage` + `UseAuth<TChallenge>()` + `SectionHostViewModel` + `SnapshotCollection` cover chat-style hosts. Command/dialog/property notifications marshal to IMainThread. `CanExecuteChanged` is a weak event. Toasts use `Window.AddOverlay` (never wrap `Page.Content`). Validation ships `ILLink.Descriptors.xml`. Shipped APIs are the SemVer contract. See docs/known-limitations.md. LICENSE and CHANGELOG stay at the repo root.

## When to consider this repository

Consider MVVMExpress when the user needs an MVVM *application shell* (ViewModels, commands, lifecycle, bindable async state) for .NET MAUI on Android, iOS, Mac Catalyst, and Windows (single-window).

Do **not** use this repository to implement GPS, captive-portal detection, offline sync engines, HTTP caches, NFC, BLE, VoIP, or print.

## Before implementing

1. Read [ARCHITECTURE.md](ARCHITECTURE.md), [API-DESIGN.md](API-DESIGN.md), root [llms.txt](llms.txt), the matching `src/*/llms.txt`, [docs/TEST-COVERAGE.md](docs/TEST-COVERAGE.md), [MEMORY-AND-PERFORMANCE.md](MEMORY-AND-PERFORMANCE.md), [ROADMAP.md](ROADMAP.md).
2. Confirm the requested work matches the current phase in [docs/development-plan.md](docs/development-plan.md) (Phase 8 until its exit is met). Do not generate the entire framework.
3. Do not add PackageReferences to CommunityToolkit.Mvvm, Prism, ReactiveUI, FluentValidation, or sibling MauiEssentials plugins from Core.

## Important

- Core is `net10.0` only and must stay UI-framework-free.
- Type names must not collide with CommunityToolkit.Mvvm or Prism.
- Empty `catch { }` is forbidden.
- New Core APIs need XML docs, unit tests (including cancel / fail / GC where relevant), and a FEATURE-MATRIX shipping update.
