# MVVMExpress Roadmap

Product: **Plugin.Maui.MVVMExpress**.

Implementation is incremental. Do not implement a later phase in the same change as an earlier one unless a Phase 1 type is blocked without it.

## Versioning

| Version | Meaning |
| --- | --- |
| 0.1.0-design | Documents + solution skeleton |
| 0.1.0-preview | Core + Host + Shell navigator + dialogs + validation + pagination (current) |
| 0.3.x | Remaining Phase 2 (page host, URI stack) |
| 0.4.x | Phase 3 — Reactive, forms, offline abstractions |
| 0.5.x | Phase 4 — Generators, restoration, compatibility |
| 1.0.0 | Phase 5 complete: tests, samples, docs, NuGet, AOT |

After 1.0.0: SemVer. Breaking API changes require a major version.

## Phase 0 — Design

- [x] ARCHITECTURE.md
- [x] API-DESIGN.md
- [x] DESIGN.md
- [x] DESIGN-PLAN.md
- [x] FEATURE-MATRIX.md
- [x] Solution / project skeleton
- [x] Memory / scale design + Core leak tests
- [ ] Design review sign-off

## Phase 1 — Core + Host (shipped in 0.1.0-preview)

**Packages:** Core, Host (`Plugin.Maui.MVVMExpress`)

- [x] ObservableModel, ViewModel, PageViewModel
- [x] Manual property notification + dependents
- [x] ModelCommand / AsyncModelCommand + generic + timeout / retry / cancel-previous
- [x] ViewModel lifecycle + cancellation + dispose (token readable after dispose)
- [x] ViewModelStatus + AsyncState\<T\> (explicit state machine still open)
- [x] Outcome / ErrorInfo
- [x] IBusyGate, IErrorSink (IOperationExecutor still open)
- [x] IMessageHub
- [x] IMainThread immediate + MauiMainThread
- [x] UseMvvmExpress / AddMvvmExpress
- [x] ViewModelLifecycleBehavior
- [x] Core unit tests including GC, cancel, fail, scale
- [x] Getting Started draft (Core)

**Exit (Core):** a test constructs a ViewModel, runs lifecycle, executes an async command into `AsyncState<T>`, cancels on dispose, and the VM is collected — **met**.

## Phase 2 — Application shell (partial in 0.1.0-preview)

**Packages:** Navigation, Dialogs, Validation, Pagination

- [x] INavigator, typed args (`IAcceptNavArgs<T>`)
- [x] Shell host (`MauiShellNavigator`); page host still open
- [x] Guards (`GuardedNavigator`, `CanNavigateAwayAsync`)
- [x] IDialogs, INotifier (`MauiDialogs` alerts; toast is Null/Fake)
- [x] DataAnnotations + IValidator
- [x] PagedCollection, refresh, SearchQuery debounce
- [x] IConnectivityProbe (in-memory; NetworkMonitor adapter documented)
- [x] FakeNavigator / FakeDialogs in Testing
- [x] Navigation tests (in-memory + Shell route/query without a window)

**Exit:** a net10.0 test navigates `Home → Details(args)` with a fake host and blocks navigation when `CanNavigateAwayAsync` returns false — **met**.

## Phase 3 — Depth

**Package:** Reactive (+ types in Core for forms)

- IPropertyObservable / CombineLatest (Rx optional)
- ICache + fetch policies (adapters, not a database)
- FormViewModel, FormField, IDirtyState, undo/redo
- File / media / permission / auth / flag abstractions
- Child ViewModel composition + scopes
- Pipeline complete (debounce, throttle, queue)

**Exit:** form with dirty guard + search-with-debounce tests pass without MAUI.

## Phase 4 — Generators and restoration

**Package:** SourceGenerators + optional Compatibility

- [Notify], command attributes
- [RegisterView] / [RegisterViewModel] / [Route]
- Deep-link mapping (sample uses Plugin.Maui.DeepLinks)
- [PersistState]
- Diagnostics (Release-off)
- CommunityToolkit adapters
- [RequiresAuth] / [RequiresRole]

**Exit:** generator snapshot tests; a trimmed sample registers views without reflection scan.

## Phase 5 — Productization

- [x] Samples: Basic, CRUD, Navigation, Auth, Offline, Pagination, Reactive, Enterprise (shared ViewModels + MAUI host; adapters are sample-local)
- BenchmarkDotNet (notify, command, collection, state, VM create)
- Full docs + migration guides (CommunityToolkit, Prism, ReactiveUI)
- AOT + trim of Enterprise sample
- Memory leak tests for navigation + messenger + behaviors
- NuGet: README, license, SourceLink, snupkg, tags
- GitHub repo, issue/PR templates already in tree
- Hub submodule + catalog row on MauiEssentials

**Exit:** Definition of Done in the master prompt — minus any item explicitly deferred in a 1.0 known-limitations section.

## Explicitly deferred past 1.0 unless review pulls them forward

- Prism-style regions
- ReactiveUI `IScreen` routing as a first-class host
- Built-in remote feature-flag or auth provider
- First-class Windows / Mac Catalyst support claims (compile TFMs may exist)
- Bottom-sheet *control* library (abstraction only in Dialogs)
- Binding debugger visualizer for Visual Studio

## Maintenance

FEATURE-MATRIX.md is updated when a phase ships. Cells stay **Designed (Pn)** until types and tests exist; then **Yes**.
