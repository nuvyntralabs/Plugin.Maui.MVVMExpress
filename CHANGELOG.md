# Changelog

All notable changes to Plugin.Maui.MVVMExpress are documented here. The format follows [Keep a Changelog](https://keepachangelog.com/). Versioning follows [SemVer](https://semver.org/) after 1.0.0.

## [0.6.0-preview] — 2026-09-01

### Fixed

- `AsyncModelCommand` / `ModelCommand` raise `CanExecuteChanged`, `IsRunning`, and `State` on `IMainThread` (Android Button animator crash)
- `ICommand.Execute` (async void) never throws; failures go to `IErrorSink` / `IDialogs`. `ExecuteAsync` still rethrows
- `MauiDialogs` hops to `IMainThread` like `MauiNotifier`
- `ObservableModel` property notifications hop when a dispatcher is present
- Toast overlay no longer unwraps page content after `ResetAsync`
- Toast uses `Window.AddOverlay` and never wraps or replaces `Page.Content`
- `ModelCommand` / `AsyncModelCommand` `CanExecuteChanged` is a weak event (Button + pop page no longer pins the page)

### Added

- `UseMvvmExpress(o => o.UseShell().UseDialogs())`, auto `ViewModelLifecycleBehavior`, `MauiShellNavigator.Map<TViewModel, TPage>` + `CreateContent`
- `FormViewModel` discard confirm, `SubmitAsync` + `MarkClean`, `FormField.Error` / `HasError`, `MustMatch`
- `GuardedNavigatorOptions` auth challenge + navigation failure forwarding
- `IAuthState.Email` / `DisplayName` / `Changed`, `IAccountService`
- Generated `[ModuleInitializer]` so `[Route]` / `[RequiresAuth]` apply from `UseMvvmExpress`
- `BusyOverlayBehavior`, `AsyncStateView`, AuthApp first-run sample, SecureSession adapter sketch
- Mac Catalyst / Windows compile TFMs (catalog-primary remains Android + iOS)
- Validation `ILLink.Descriptors.xml` roots `Required` / `MinLength` / `MustMatch` and the other 0.6 DataAnnotations

## [0.5.0-preview] — 2026-09-01

### Added

- `[Notify]`, `[NotifyAlso]`, `[ModelCommand]`, `[AsyncModelCommand]`, `[RegisterView]` / `[RegisterViewModel]` / `[Route]`, `[PersistState]`, `[RequiresAuth]` / `[RequiresRole]`
- Source generator emits properties, commands, persist methods, and `MvvmExpressGeneratedRegistrations` (no reflection scan)
- `IStateStore` / `MemoryStateStore`, `INavigationAuthPolicy`, `IRoleState`, `IMvvmExpressDiagnostics` (Release-off)
- `Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit` (`CommunityToolkitMessageHub`)
- Sample `GeneratedCatalogViewModel`, `DeepLinkRouteMap`, and `AddGeneratedViewModels()`
- Migration guides (CommunityToolkit, Prism, ReactiveUI), AOT/trim notes, known limitations
- Testing fakes, `ViewModelLifecycle`, `ScopedNavigator`, page-scope sample, pop-GC tests, remaining BenchmarkDotNet jobs

### Changed

- Version is `0.5.0-preview`. Phases 4–5 ship on top of released `0.4.0-preview`. Shipped APIs are the 1.0 contract; 1.0.0 waits on design-review sign-off.

## [0.4.0-preview] — 2026-09-01

### Added

- `FormViewModel`, `FormField<T>`, `IDirtyState`, `UndoStack` — dirty navigation guard + undo / redo
- `IPropertyObservable<T>` / `PropertyObservable.CombineLatest` in the Reactive package (no System.Reactive)
- `ICachedFetcher` / `CachedFetcher` with `FetchPolicy` (cache-first, network-first, SWR)
- `IOperationExecutor` — shared timeout / retry / debounce / throttle / queue pipeline
- Command `ConcurrencyMode.Queue` / `Allow` / `Replace` plus `Debounce` / `Throttle` on `AsyncCommandOptions`
- Child ViewModel composition (`IViewModelComposer.Attach`) and `IViewModelScopeFactory`
- Abstractions: `IFeatureSwitch`, `IPermissionGate`, `IFileStore`, `IMediaPicker`
- Reactive package is packable. Sample Edit page shows dirty / undo / redo.

### Changed

- Version is `0.4.0-preview`. Phase 3 (forms, reactive, cache policies, pipeline, scopes) ships with tests.

## [0.3.0-preview] — 2026-09-01

### Added

- `MauiPageNavigator` / `IPageNavigator` — page-stack host on `INavigation` / `NavigationPage`
- URI stack on `INavigator`: `Stack`, `ModalStack`, `CanGoBack`, `PopToRootAsync`, `ReplaceAsync`, `ResetAsync`
- Dictionary / URI navigation: `NavigateToAsync(route, query)`, `IAcceptNavQuery`, `NavigationRouteTable`
- Multi-window root: `IWindowContext`, `WindowNavigatorRegistry`, `MauiWindowContext`
- `MauiNotifier` toast overlay (`IToastPresenter` for tests)
- Sample flyout **Page stack** plus Home URI query and toast buttons

### Changed

- Version is `0.3.0-preview`. Phase 2 leftovers (page host, URI stack, toast, multi-window) are shipped with tests.

## [0.1.1-preview] — 2026-08-31

### Changed

- NuGet `RepositoryUrl` is [nuvyntralabs/Plugin.Maui.MVVMExpress](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress) (0.1.0-preview still pointed at NiladriPadhy).

## [0.1.0-preview] — 2026-08-31

### Added

- `AddMvvmExpress` / `UseMvvmExpress`, `MauiMainThread`, `ViewModelLifecycleBehavior`
- `INavigator`, `InMemoryNavigator`, `GuardedNavigator`, `IAcceptNavArgs<T>`, `PageViewModel`
- `MauiShellNavigator` (ViewModel → Shell route + query args)
- `IDialogs` / `INotifier`, `NullDialogs`, `MauiDialogs`, `FakeDialogs`
- `IValidator` / `DataAnnotationsValidator`, `PagedCollection<T>`, `SearchQuery`
- `ICache`, `IConnectivityProbe`, `IAuthState`, command timeout / retry / cancel-previous
- Sample MAUI host: Home → CRUD flyout, Home → product details page, login → secure page

### Changed

- Version is `0.1.0-preview`. README no longer describes the product as production-ready.
- Samples use library types (not sample-local navigator/cache/connectivity fakes).
- Each packed library has its own README and `llms.txt`. LICENSE and CHANGELOG stay at the repo root.

### Notes

- Packable packages: Core, Host, Navigation, Dialogs, Validation, Pagination, Testing. SourceGenerators are not packed yet.
- Device RSS / AOT / trim are still Phase 5 work.
- In-memory auth / cache / connectivity are demo adapters. Production: Plugin.Maui.SecureSession, ApiCache / OfflineSync, NetworkMonitor.

## [0.1.0-design] — 2026-08-31

### Added

- Architecture, API design, product design, design-detail plan, roadmap, feature matrix, and [MEMORY-AND-PERFORMANCE.md](MEMORY-AND-PERFORMANCE.md)
- Modular solution (Core, Host, Navigation, Dialogs, Validation, Pagination, Reactive, SourceGenerators, Testing)
- Core runtime slice: `ObservableModel`, `ViewModel`, commands, `AsyncState<T>`, `MessageHub`, `ObservableRangeCollection<T>`
- Leak probes and Small / Mid / Large scale tests and BenchmarkDotNet jobs
- Open-source files (LICENSE, CONTRIBUTING, CODE_OF_CONDUCT, SECURITY)

### Changed

- Library name is **MVVMExpress** / `Plugin.Maui.MVVMExpress`

### Fixed

- `ViewModelCancellationToken` stays readable after `Dispose`
- `AsyncModelCommand<T>` releases its single-flight lock if linked CTS construction fails
