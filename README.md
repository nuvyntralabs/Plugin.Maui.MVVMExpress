# Plugin.Maui.MVVMExpress

A modular MVVM framework for .NET MAUI (ViewModels, commands, async state, Shell **or** NavigationPage, dialogs, validation, pagination).

**Product name:** MVVMExpress (MVVM + Express)  
**Package prefix:** `Plugin.Maui.MVVMExpress`  
**Status:** `1.0.0` — SemVer lock (`UseAuth<TChallenge>()`, 15-minute path, Playground). **Supported:** Android, iOS, Mac Catalyst, and Windows (single-window). Host APIs are shared MAUI — no platform stub. Shipped public APIs in [API-DESIGN.md](API-DESIGN.md) are the contract. See [known limitations](docs/known-limitations.md). [Getting started](docs/getting-started.md) · [Chat host](docs/chat-host.md) · [Android do/don't](docs/maui-android.md).

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Core.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Core)

[Technical documentation](https://nuvyntralabs.github.io/packages/plugin-maui-mvvmexpress/) · [Architecture](ARCHITECTURE.md) · [API design](API-DESIGN.md) · [Getting started](docs/getting-started.md) · [Development plan](docs/development-plan.md) · [Navigation](docs/navigation.md) · [Chat host](docs/chat-host.md) · [Forms](docs/forms.md) · [Reactive](docs/reactive.md) · [Memory & performance](MEMORY-AND-PERFORMANCE.md) · [Test coverage](docs/TEST-COVERAGE.md) · [Feature matrix](FEATURE-MATRIX.md)

Author: [Niladri Prasad Padhy](https://github.com/NiladriPadhy) · Catalog: [MauiEssentials](https://github.com/nuvyntralabs/MauiEssentials) · License: MIT

Sample: [WhatsApp clone using MVVMExpress](https://github.com/nuvyntralabs/WhatsAppUIClone)

## Why this exists

CommunityToolkit.Mvvm covers properties, commands, and messaging. Prism.Maui covers page navigation and dialogs (not Shell). ReactiveUI covers observable pipelines. A production MAUI app often needs all three *plus* bindable async state, lifecycle-aware cancellation, and typed navigation — without taking three overlapping frameworks.

MVVMExpress is that shell. It is **not** a fork of those libraries. Capability work (captive portal, HTTP cache, offline sync, form XAML, flags, deep links) stays in focused [MauiEssentials](https://github.com/nuvyntralabs/MauiEssentials) plugins.

## Packages

| Package | Purpose | Status |
| --- | --- | --- |
| [`Plugin.Maui.MVVMExpress.Core`](src/Plugin.Maui.MVVMExpress.Core/README.md) | Observable model, commands, ViewModel, navigator/cache/auth/connectivity abstractions, state, outcome, messaging | **Implemented + tests** |
| [`Plugin.Maui.MVVMExpress.Testing`](src/Plugin.Maui.MVVMExpress.Testing/README.md) | `LeakProbe`, `ScaleProfile`, fakes, `AppearAsync`, `ScopedNavigator` | **Implemented + tests** |
| [`Plugin.Maui.MVVMExpress`](src/Plugin.Maui.MVVMExpress/README.md) | `UseMvvmExpress`, `MauiMainThread`, page lifecycle behavior | **Implemented** |
| [`Plugin.Maui.MVVMExpress.Navigation`](src/Plugin.Maui.MVVMExpress.Navigation/README.md) | `UseShell` **or** `UseNavigationPage`; pages constructed on `IMainThread` | **Implemented + tests** |
| [`Plugin.Maui.MVVMExpress.Dialogs`](src/Plugin.Maui.MVVMExpress.Dialogs/README.md) | `IDialogs` + `MauiDialogs` + `MauiNotifier` (`Window.AddOverlay` toast) | **Implemented + tests** |
| [`Plugin.Maui.MVVMExpress.Validation`](src/Plugin.Maui.MVVMExpress.Validation/README.md) | DataAnnotations + `IValidator` + trim descriptor | **Implemented + tests** |
| [`Plugin.Maui.MVVMExpress.Pagination`](src/Plugin.Maui.MVVMExpress.Pagination/README.md) | `PagedCollection<T>`, `SnapshotCollection<T>`, `SearchQuery` (`CommittedText`) | **Implemented + tests** |
| [`Plugin.Maui.MVVMExpress.Reactive`](src/Plugin.Maui.MVVMExpress.Reactive/README.md) | `IPropertyObservable` / `CombineLatest` (no Rx required) | **Implemented + tests** |
| [`Plugin.Maui.MVVMExpress.SourceGenerators`](src/Plugin.Maui.MVVMExpress.SourceGenerators/README.md) | `[Notify]`, commands, register, routes, persist, auth | **Implemented + snapshot tests** |
| [`Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit`](src/Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit/README.md) | `IMessenger` → `IMessageHub` adapter | **Implemented + tests** |

Each packed package ships its own README on nuget.org. This file is the product index. License and changelog stay at the repo root.

Core targets `net10.0` and does not reference MAUI.

## Core usage (implemented)

```csharp
public sealed class HomeViewModel : ViewModel
{
    public AsyncState<IReadOnlyList<Product>> Products { get; } = new();

    public AsyncModelCommand RefreshCommand { get; }

    public HomeViewModel(ICatalog catalog)
    {
        RefreshCommand = new AsyncModelCommand(
            ct => Products.LoadAsync(token => catalog.ListAsync(token), ct));
    }
}
```

```csharp
items.AddRange(page); // one CollectionChanged — required for mid/large lists
hub.Subscribe<HomeViewModel, RefreshMsg>(this, static (vm, _) => vm.Refresh());
```

```bash
dotnet add package Plugin.Maui.MVVMExpress.Core
dotnet add package Plugin.Maui.MVVMExpress
```

Register Core services with `services.AddMvvmExpress()`. In a MAUI host call `builder.UseMvvmExpress()`. See [docs/getting-started.md](docs/getting-started.md).

## Feature comparison

Designed product surface, validated 2026-08-31 against CommunityToolkit.Mvvm 8.4, Prism.Maui 9, and ReactiveUI. Shipping vs designed: [FEATURE-MATRIX.md](FEATURE-MATRIX.md).

| Feature | MVVMExpress | CommunityToolkit.Mvvm | Prism.Maui | ReactiveUI |
| --- | --- | --- | --- | --- |
| Observable properties | Yes | Yes | Yes | Yes |
| Commands / async commands | Yes | Yes | Yes / Partial | Yes |
| Source generators | Yes (`[Notify]`, commands, register, routes) | Yes | No | Yes |
| Navigation (Shell **or** page) | Yes (`MauiShellNavigator` + `MauiPageNavigator`) | No | Yes (page only; no Shell) | Yes |
| Lifecycle + cancellation | Yes | No | Yes | Yes |
| Dialogs / in-app notifications | Yes | Separate | Yes | Extensions |
| Validation | Yes | Yes | Extensions | Yes |
| Reactive derived state | Yes (`CombineLatest`; Rx optional) | No | No | Yes (Rx required) |
| Pagination + refresh + search | Yes | No | Extensions | Extensions |
| Offline / cache abstractions | Yes (adapters; not a database) | No | No | Extensions |
| Unified `AsyncState<T>` | Yes | No | No | Extensions |
| Typed navigation `record` args | Yes | No | No (dictionary / URI) | Partial |
| Memory-leak GC tests (VM, command, Button pop, messenger) | Yes | Partial | Partial | Partial |
| Small / mid / large list batching | Yes (`AddRange` one notify) | App code | App code | App / Rx |
| Testing package | Yes | Partial | Yes | Yes |

This table does not claim MVVMExpress is faster than the others. Measured Core numbers are in [MEMORY-AND-PERFORMANCE.md](MEMORY-AND-PERFORMANCE.md).

**Shipped in this repo with tests:** properties, commands (prevent / cancel-previous / queue / allow, timeout, retry, debounce, throttle, weak `CanExecuteChanged`), ViewModel lifecycle/dispose/`ExecuteAsync`, `AsyncState<T>`, `Outcome`, `BusyGate`, `MessageHub`, `ObservableRangeCollection<T>`, `INavigator` / `GuardedNavigator` / `MauiShellNavigator` / `IAcceptNavArgs<T>`, `IDialogs`, `Window.AddOverlay` toasts, `ICache` / `ICachedFetcher`, `FormViewModel`, `IOperationExecutor`, `IPropertyObservable`, `IConnectivityProbe`, `IAuthState`, `IValidator` + Validation trim roots, `PagedCollection<T>`, `SearchQuery`, `AddMvvmExpress` / `UseMvvmExpress`, leak probes (including Button + pop page), `ScopedNavigator` pop-GC, Small/Mid/Large scale tests.

## Memory, leaks, and scale

| Scale | List size | Guarantee |
| --- | --- | --- |
| Small | 200 | Cheap notify; per-item `Add` is acceptable |
| Mid | 5_000 | `AddRange` → one `Reset` |
| Large | 50_000 | Same batching; UI must virtualize |

```bash
dotnet test tests/Plugin.Maui.MVVMExpress.Core.Tests
dotnet run --project benchmarks/Plugin.Maui.MVVMExpress.Benchmarks -c Release
```

## Related MauiEssentials packages

- [Plugin.Maui.NetworkMonitor](https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor) — real internet vs captive portal
- [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) — `Validation.For` XAML
- [Plugin.Maui.OfflineSync](https://www.nuget.org/packages/Plugin.Maui.OfflineSync) / [Plugin.Maui.ApiCache](https://www.nuget.org/packages/Plugin.Maui.ApiCache)
- [Plugin.Maui.FeatureFlags](https://www.nuget.org/packages/Plugin.Maui.FeatureFlags), [Plugin.Maui.DeepLinks](https://www.nuget.org/packages/Plugin.Maui.DeepLinks), [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession)
- [Plugin.Maui.Diagnostics](https://www.nuget.org/packages/Plugin.Maui.Diagnostics) — ANR / crash breadcrumbs
- [Plugin.Maui.KeyboardManager](https://www.nuget.org/packages/Plugin.Maui.KeyboardManager) — composer keyboard pan / dismiss

These are Niladri Padhy / MauiEssentials / Nuvyntra Labs packages. Usual alternatives: MAUI `Connectivity`, CommunityToolkit.Maui, Polly.

## Development

```bash
dotnet build Plugin.Maui.MVVMExpress.slnx
dotnet test Plugin.Maui.MVVMExpress.slnx
dotnet test tests/Plugin.Maui.MVVMExpress.Samples.Tests
```

## Pack from source

Same pattern as the other MauiEssentials plugins (`dotnet pack` → `artifacts/`).

```bash
dotnet pack src/Plugin.Maui.MVVMExpress.Core/Plugin.Maui.MVVMExpress.Core.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress/Plugin.Maui.MVVMExpress.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.Navigation/Plugin.Maui.MVVMExpress.Navigation.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.Dialogs/Plugin.Maui.MVVMExpress.Dialogs.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.Validation/Plugin.Maui.MVVMExpress.Validation.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.Pagination/Plugin.Maui.MVVMExpress.Pagination.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.Testing/Plugin.Maui.MVVMExpress.Testing.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.Reactive/Plugin.Maui.MVVMExpress.Reactive.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.SourceGenerators/Plugin.Maui.MVVMExpress.SourceGenerators.csproj -c Release -o artifacts
dotnet pack src/Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit/Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit.csproj -c Release -o artifacts
```

Publish (requires a nuget.org API key; siblings are packed and pushed this way, not via a workflow in-repo):

```bash
dotnet nuget push artifacts/*.nupkg --source https://api.nuget.org/v3/index.json --api-key $NUGET_API_KEY --skip-duplicate
```

Samples: [samples/README.md](samples/README.md) · [WhatsApp clone using MVVMExpress](https://github.com/nuvyntralabs/WhatsAppUIClone)
