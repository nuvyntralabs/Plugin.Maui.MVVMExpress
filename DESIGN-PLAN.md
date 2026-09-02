# MVVMExpress Design Detail Plan

Work plan for implementing [ARCHITECTURE.md](ARCHITECTURE.md) and [API-DESIGN.md](API-DESIGN.md). This is not a license to generate the entire framework in one change.

**Stop condition:** Phases 0–7 are shipped. Post-1.0 work lives in [docs/development-plan.md](docs/development-plan.md) (Phase 8). Do not regenerate the whole framework in one change. Accepted 1.0 scope is in [docs/known-limitations.md](docs/known-limitations.md).

## 1. Current milestone

| Item | Status |
| --- | --- |
| Architecture | Living contract — status banner updated for 0.5.0 |
| API design | Living contract — shipped vs proposed distinguished in the header |
| Roadmap / feature matrix | Current — 1.0.0 (Phases 6–7). Next is Phase 8 |
| Solution + packages | Core, Host, Navigation, Dialogs, Validation, Pagination, Reactive, Testing, SourceGenerators, Compatibility packed |
| Core runtime | Implemented + generators / persist / auth / diagnostics tests. Host timings in MEMORY-AND-PERFORMANCE §2.1 |
| Phase 4 generators | Shipped: `[Notify]`, commands, registration, persist, auth, `AddGeneratedViewModels` |
| Phase 5 productization | Migration guides, AOT/trim sample, Testing fakes, hub catalog, NuGet polish |

## 2. Decision log

| ID | Decision | Status |
| --- | --- | --- |
| D1 | Official name **MVVMExpress**; package prefix `Plugin.Maui.MVVMExpress` | Accepted |
| D2 | Core has zero MAUI / Rx / CommunityToolkit / Prism / FluentValidation references | Proposed |
| D3 | Type names avoid CT/Prism collisions (`ObservableModel`, `INavigator`, `Outcome`) | Proposed |
| D4 | Microsoft.Extensions.DependencyInjection only | Proposed |
| D5 | Shell and page navigation are both first-class; neither is required | Proposed |
| D6 | Sibling MauiEssentials plugins are adapters, never PackageReferences | Proposed |
| D7 | Source generators ship in Phase 4; Phase 1 is handwritten APIs | Proposed |
| D8 | `UseMvvmExpress` + `AddMvvmExpress` (not `AddMauiMvvm`) | Proposed |
| D9 | Android + iOS primary; Mac Catalyst + Windows compile TFMs | Proposed |
| D10 | Diagnostics off by default in Release | Proposed |

## 3. Phase 1 — Core + Host (after review)

### 3.1 Deliverables

- `ObservableModel` (INPC / INPChanging, `SetProperty`, dependents)
- `ModelCommand` / `AsyncModelCommand` (+ generic), `AsyncCommandOptions` (concurrency, cancel previous, timeout, retry — debounce/throttle may be Phase 3 if timeboxed)
- `ViewModel` / `PageViewModel` lifecycle + `ViewModelCancellationToken` + dispose
- `ViewModelStatus`, `AsyncState<T>`, optional transition table
- `Outcome` / `Outcome<T>`
- `IBusyGate`, `IErrorSink`, `IOperationExecutor` (minimal)
- `IMessageHub` (weak + strong)
- `IMainThread` abstraction + MAUI implementation
- `AddMvvmExpress` / `UseMvvmExpress`
- Lifecycle behavior
- Unit tests: property, command (async, cancel, concurrency), state, outcome, GC of ViewModel, no-empty-catch
- XML docs on public types
- Treat warnings as errors on Core + Host

### 3.2 Out of scope for Phase 1

Navigation, dialogs, validation package, pagination, reactive, generators, samples beyond a compile-only stub, NuGet publish, Prism/CT compatibility package.

### 3.3 First files (indicative)

```
src/Plugin.Maui.MVVMExpress.Core/
  ComponentModel/ObservableModel.cs
  ComponentModel/ViewModel.cs
  ComponentModel/PageViewModel.cs
  ComponentModel/IAsyncLifecycle.cs
  Input/ModelCommand.cs
  Input/AsyncModelCommand.cs
  Input/AsyncCommandOptions.cs
  State/ViewModelStatus.cs
  State/AsyncState.cs
  State/StateMachine.cs
  Outcome/Outcome.cs
  Messaging/IMessageHub.cs
  Operations/IOperationExecutor.cs
  Busy/IBusyGate.cs
  Errors/IErrorSink.cs
  Threading/IMainThread.cs
  Collections/ObservableRangeCollection.cs   (may slip to Phase 2)

src/Plugin.Maui.MVVMExpress/
  Hosting/MVVMExpressServiceCollectionExtensions.cs
  Hosting/MvvmExpressOptions.cs
  Lifecycle/ViewModelLifecycleBehavior.cs
  Threading/MauiMainThread.cs
```

### 3.4 Phase 1 acceptance

- `dotnet test` on Core.Tests passes on `net10.0`
- Host builds for `net10.0`, `net10.0-android`, `net10.0-ios` (Catalyst/Windows as configured)
- A ViewModel can be constructed in a test, `InitializeAsync` / `OnAppearingAsync` run, command executes, `AsyncState` transitions Idle → Loading → Success, dispose cancels the token
- `WeakReference` test: disposed ViewModel is collected
- No `Microsoft.Maui.Controls` reference on Core (`dotnet list package` / project file assert)

## 4. Phase 2 — Application shell

1. `INavigator` + typed args + dictionary interop
2. `ShellNavigationHost` and `PageNavigationHost`
3. Guards, events, back interception
4. `IDialogs` + `INotifier` (toast/snackbar abstractions; platform implementations may be thin MAUI wrappers)
5. Validation package (DataAnnotations + custom; FluentValidation adapter interface only)
6. Pagination + refresh + search (debounce, cancel previous)
7. `IConnectivityProbe` (default: MAUI `IConnectivity`; documented adapter for NetworkMonitor)
8. Navigation + dialog unit tests with fakes (no device)

## 5. Phase 3 — Depth

1. Reactive package (`IPropertyObservable<T>`, `CombineLatest`, optional Rx)
2. Cache / offline *abstractions* + sample adapter to ApiCache (no DB)
3. Forms, dirty, undo
4. File/media/permission/auth/flag abstractions
5. Full retry / timeout / pipeline polish
6. Child ViewModel composition + scope factory

## 6. Phase 4 — Generators and restoration

1. Source generator package + snapshot tests
2. `[RegisterView]` / `[RegisterViewModel]` / `[Route]`
3. Deep-link map (compose DeepLinks plugin in sample, not in Core)
4. `[PersistState]`
5. Diagnostics / binding diagnostics
6. CommunityToolkit compatibility adapters
7. Authorization attributes

## 7. Phase 5 — Productization

1. Samples 1–8
2. Testing package completeness
3. BenchmarkDotNet
4. Docs listed in the master prompt
5. AOT / trim publish of a sample
6. NuGet (README, license, SourceLink, snupkg)
7. GitHub repo + submodule on MauiEssentials hub
8. Migration guides (CT, Prism, ReactiveUI)

## 8. Test strategy

| Layer | Framework | Notes |
| --- | --- | --- |
| Core.Tests | xUnit, `net10.0` | No MAUI. Async, cancel, concurrency, GC |
| Navigation.Tests | xUnit + fakes | Serialize navigations, guard cancel |
| Dialogs.Tests | xUnit + `FakeDialogs` | |
| Validation.Tests | xUnit | DataAnnotations + custom |
| Reactive.Tests | xUnit | No UI |
| Generator.Tests | Microsoft.CodeAnalysis.Testing | Snapshot |
| Integration.Tests | MAUI host when available | Lifecycle, DI, behavior — Phase 2+ |
| Benchmarks | BenchmarkDotNet | Phase 5 |

Memory tests: create, subscribe, dispose, `GC.Collect()`, assert `WeakReference.IsAlive == false`.

## 9. Documentation plan

| Doc | When |
| --- | --- |
| Architecture / API / roadmap / matrix | Now (this drop) |
| Getting started | End of Phase 1 |
| Navigation / dialogs / validation | End of Phase 2 |
| Reactive / offline / forms | End of Phase 3 |
| Generators / migration / AOT | End of Phase 4 |
| Samples + API reference | Phase 5 |

## 10. Review checklist (before any Phase 1 feature commit)

- [x] Package prefix approved (`Plugin.Maui.MVVMExpress`)
- [ ] `Outcome` vs `Result` naming approved
- [ ] Collision names approved (`ObservableModel`, `INavigator`, …)
- [ ] Phase 1 file list approved (especially whether collections slip)
- [ ] Confirm Host auto-lifecycle vs XAML-only behavior
- [ ] Confirm no CommunityToolkit PackageReference in Phase 1
