# Changelog

All notable changes to Plugin.Maui.MVVMExpress are documented here. The format follows [Keep a Changelog](https://keepachangelog.com/). Versioning follows [SemVer](https://semver.org/) after 1.0.0.

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

- Packable packages: Core, Host, Navigation, Dialogs, Validation, Pagination, Testing. Reactive and SourceGenerators are not packed yet.
- No page-stack host, source generators, or Reactive package.
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
