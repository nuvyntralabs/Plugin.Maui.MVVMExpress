# MVVMExpress Architecture

**Product:** MVVMExpress  
**Official package family:** `Plugin.Maui.MVVMExpress.*`  
**Status:** `0.5.0-preview`. Core through Reactive, source generators, persist/auth attributes, CommunityToolkit adapter, and Phase 5 productization (samples, Testing fakes, migration guides, AOT/trim notes) are shipped with tests. Shipped public APIs are the 1.0 contract; 1.0.0 waits on design-review sign-off. See [FEATURE-MATRIX.md](FEATURE-MATRIX.md), [ROADMAP.md](ROADMAP.md), and [docs/known-limitations.md](docs/known-limitations.md).

MVVMExpress is a modular MVVM application framework for .NET MAUI. It is not a fork of CommunityToolkit.Mvvm, Prism.Maui, or ReactiveUI. Those libraries are studied as capability references. This document records the original architecture that delivers equivalent developer outcomes without copying their type graphs, containers, or navigation engines.

Sibling MauiEssentials plugins already solve connectivity, offline sync, HTTP cache, form validation, feature flags, deep links, permissions, clipboard, share, and auth sessions. MVVMExpress **composes** those plugins through adapters. It does not reimplement them.

## 1. Problem

A production MAUI app needs more than `INotifyPropertyChanged` and `ICommand`:

- ViewModel lifecycle bound to page / window / process lifetime
- Async work with cancellation, timeout, retry, concurrency, and busy state
- Strongly typed navigation that does not require a dictionary
- UI state that is richer than a boolean `IsBusy`
- Testable ViewModels with no static MAUI calls
- Optional reactive derived state without forcing System.Reactive on every app

CommunityToolkit.Mvvm covers properties, commands, and messaging. Prism.Maui covers page navigation, dialogs, and DI registration. ReactiveUI covers observable pipelines and activation. None of them, alone, is a MAUI-first operation + state + scope framework. Combining all three creates package, namespace, and mental-model collisions.

MVVMExpress is the application shell. Capability plugins remain optional.

## 2. Design principles

1. **Core is UI-framework-free.** `Plugin.Maui.MVVMExpress.Core` targets `net10.0` only and must not reference `Microsoft.Maui.Controls`.
2. **Optional means optional.** Navigation, dialogs, validation, reactive, pagination, and generators are separate packages.
3. **Interfaces at the ViewModel boundary.** ViewModels depend on `INavigator`, `IDialogs`, `IMainThread`, `IConnectivityProbe` — never on `Shell`, `Page.DisplayAlert`, or `MainThread` statics.
4. **Async-first, cancellation-first.** Every public async API accepts `CancellationToken`. Blocking `.Result` / `.Wait()` is forbidden in library code.
5. **No hidden global state.** No static service locator in Core. An optional host accessor may exist for XAML / design-time only and is off by default in tests.
6. **Compose MauiEssentials.** Connectivity, cache, offline, permissions, flags, deep links, and secure session are adapter surfaces, not new engines.
7. **Source generators are an accelerator, not a requirement.** Manual `INotifyPropertyChanged` and hand-written commands remain first-class.
8. **AOT and trimming are default constraints.** Reflection-based registration is a debug/convention fallback, never the only path.
9. **Do not silently swallow exceptions.** Every catch either transforms to `Outcome`, calls `IErrorSink`, logs and rethrows, or is documented as intentional (for example, `OperationCanceledException` mapped to `Cancelled`).
10. **One window is not the app.** Navigation, dialogs, and scopes are keyed by `IWindowContext`, not `Application.Current.MainPage`.

## 3. Naming

| Role | Value |
| --- | --- |
| Product | MVVMExpress (MVVM + Express) |
| NuGet / assembly prefix | `Plugin.Maui.MVVMExpress` |
| Root namespace | `Plugin.Maui.MVVMExpress` |
| Host registration | `builder.UseMvvmExpress(...)` / `services.AddMvvmExpress(...)` |
| Catalog slug | `plugin-maui-mvvmexpress` |

Published packages follow the MauiEssentials `Plugin.Maui.*` convention so they sit next to GeoLocator, FormValidation, and the rest of the catalog.

Core type names **must not collide** with CommunityToolkit.Mvvm or Prism when both are referenced:

| Concept | MVVMExpress | Avoid copying |
| --- | --- | --- |
| Observable base | `ObservableModel` | `ObservableObject` |
| ViewModel base | `ViewModel` / `PageViewModel` | Prism `BindableBase` |
| Sync command | `ModelCommand` | `RelayCommand`, `DelegateCommand` |
| Async command | `AsyncModelCommand` | `AsyncRelayCommand`, `ReactiveCommand` |
| Notify attribute | `[Notify]` | `[ObservableProperty]` |
| Command attribute | `[ModelCommand]`, `[AsyncModelCommand]` | `[RelayCommand]` |
| Messenger | `IMessageHub` | `IMessenger`, `IEventAggregator` |
| Navigation | `INavigator` | Prism `INavigationService` |
| Dialogs | `IDialogs` | Prism `IDialogService` |
| Parameters | `NavArgs` / typed records | `INavigationParameters` |
| Result | `Outcome` / `Outcome<T>` | competing `Result<T>` packages |

Compatibility adapters (shipped in `0.5.0-preview`) map CommunityToolkit types onto MVVMExpress types. They do not type-forward the same names.

## 4. Package architecture

```
Plugin.Maui.MVVMExpress.Core                 net10.0
        ▲
        │
Plugin.Maui.MVVMExpress                      MAUI host (DI, lifecycle, dispatcher, locator)
        ▲
        ├── Navigation
        ├── Dialogs
        ├── Validation
        ├── Pagination
        └── Reactive

Plugin.Maui.MVVMExpress.SourceGenerators     netstandard2.0 analyzer (no runtime MAUI)
Plugin.Maui.MVVMExpress.Testing              net10.0 fakes
```

| Package | TFMs | MAUI? | Role |
| --- | --- | --- | --- |
| `Plugin.Maui.MVVMExpress.Core` | `net10.0` | No | Observable model, commands, ViewModel, state, outcome, messaging, busy, retry, timeout, collections, selection, dirty, undo, task tracking |
| `Plugin.Maui.MVVMExpress` | `net10.0` + platform TFMs | Yes | `AddMvvmExpress`, lifecycle behaviors, main-thread dispatcher, ViewModel resolver, window context, app lifecycle bridge |
| `Plugin.Maui.MVVMExpress.Navigation` | MAUI TFMs | Yes | `INavigator`, Shell + page hosts, guards, stacks, typed args, deep-link mapping |
| `Plugin.Maui.MVVMExpress.Dialogs` | MAUI TFMs | Yes | Alerts, confirm, input, action sheet, loading, toast/snackbar abstractions |
| `Plugin.Maui.MVVMExpress.Validation` | `net10.0` | No | DataAnnotations, custom validators, optional FluentValidation adapter |
| `Plugin.Maui.MVVMExpress.Pagination` | `net10.0` | No | Page/cursor lists, load-more, refresh, search |
| `Plugin.Maui.MVVMExpress.Reactive` | `net10.0` | No | Property streams, derived state, reactive commands — **no System.Reactive in Core** |
| `Plugin.Maui.MVVMExpress.SourceGenerators` | `netstandard2.0` | No | `[Notify]`, commands, register, route |
| `Plugin.Maui.MVVMExpress.Testing` | `net10.0` | No | Fake navigator, dialogs, dispatcher, connectivity, messenger |

**Not separate packages in v1** (types live in Core or Host, enabled by options):

- Result / outcome
- State machine
- Busy / error / retry / timeout
- Collections / selection
- Forms / dirty / undo
- Connectivity *abstractions* (implementation in Host)
- Cache *abstractions* (adapters later)
- Auth / feature-flag *abstractions*
- File / media / permission *abstractions*

**Later optional packages (Phase 3–4, only if Core would otherwise grow too large):**

- `Plugin.Maui.MVVMExpress.Offline` — cache + fetch policies; adapters for ApiCache / OfflineSync
- `Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit` — type adapters, not a fork

## 5. Dependency graph

```
                    ┌──────────────────────────────────────┐
                    │  Application (MAUI host)             │
                    │  UseMvvmExpress / AddMvvmExpress       │
                    └──────────────────┬───────────────────┘
                                       │
         ┌──────────────┬──────────────┼──────────────┬──────────────┐
         ▼              ▼              ▼              ▼              ▼
    Navigation      Dialogs      Validation     Pagination      Reactive
         │              │              │              │              │
         └──────────────┴──────┬───────┴──────────────┴──────────────┘
                               ▼
                         Host package
                     (lifecycle, DI, dispatcher)
                               │
                               ▼
                              Core
              (no MAUI, no Rx, no FluentValidation, no Prism)
                               │
         optional adapters ────┼──── optional sibling plugins
                               ▼
         NetworkMonitor · ApiCache · OfflineSync · FormValidation
         FeatureFlags · DeepLinks · PermissionFlow · SecureSession
         ClipboardPlus · SharePlus · Diagnostics
```

**Hard rules**

- Core → nothing MAUI, nothing Rx, nothing FluentValidation, nothing CommunityToolkit, nothing Prism.
- Host → Core + `Microsoft.Maui.Controls` + `Microsoft.Extensions.DependencyInjection` + `Microsoft.Extensions.Logging.Abstractions`.
- Navigation → Host (for window/page resolution).
- Dialogs → Host.
- Validation → Core only. FluentValidation is an optional package reference the *app* adds; MVVMExpress.Validation detects it via adapter interface, not a PackageReference.
- Reactive → Core. `System.Reactive` is an *optional* PackageReference of the Reactive package, isolated behind `IPropertyObservable<T>` so Core never takes the dependency.
- SourceGenerators → Roslyn only. Attributes live in Core so apps can compile without the generator package (attributes become no-ops).
- Testing → Core. Additional fakes for Navigation/Dialogs are provided as source-only or optional InternalsVisibleTo surfaces, not as a Testing → Navigation package cycle. Testing may reference Navigation/Dialogs *abstractions* that live in Core.

Sibling plugins are **never** PackageReferences of MVVMExpress packages. Apps (or a thin adapter in the host app) wire them.

## 6. Layering

```
View (Page / Shell / XAML)
        │ owns BindingContext
        │ behaviors bind lifecycle
        ▼
ViewModel (PageViewModel)
        │ INavigator, IDialogs, IErrorSink, IBusyGate
        │ Outcome / AsyncState / commands
        ▼
Application services (interfaces)
        │ repositories, caches, auth, flags
        ▼
Adapters  ──►  MAUI Essentials / MauiEssentials plugins / app backends
```

This matches the intent of the four reference diagrams (navigator creates VC+VM; View owns VM and binds; VM owns and updates model; store sits between disk and network) without adopting any one vendor’s type names.

### 6.1 ViewModel ownership

- The **Page owns the ViewModel instance** for a page scope (MAUI `BindingContext`).
- The **navigator creates** the page and resolves the ViewModel from the current `IServiceScope`.
- The ViewModel **does not** create pages.
- Child ViewModels are created by the parent scope (`IViewModelComposer`) and receive propagated lifecycle.

### 6.2 Data flow

```
User event  →  ICommand / binding
     →  AsyncModelCommand  →  OperationPipeline
           →  Outcome<T> / AsyncState<T>
           →  PropertyChanged
           →  View update

Navigate    →  INavigator  →  guards  →  host (Shell or INavigation)
           →  INavigable.OnNavigatedToAsync
```

## 7. Core subsystems

### 7.1 Observable model

`ObservableModel` implements `INotifyPropertyChanged` and `INotifyPropertyChanging`. Manual `SetProperty`, `SetPropertyAndNotify`, dependent-property lists, and equality checks are in Core.

`[Notify]` (generator) emits a public property, `OnXChanging` / `OnXChanged` partials, and optional `[NotifyAlso(nameof(FullName))]`.

### 7.2 Commands

`ModelCommand` / `AsyncModelCommand` (and generic variants) sit on an **operation pipeline**:

```
CanExecute → concurrency gate → timeout → retry → execute
         → progress / IsRunning → error sink → Outcome
         (debounce/throttle on the command itself remain later; SearchQuery already debounces search)
```

Shipped concurrency modes: `Prevent`, `CancelPrevious`. Designed later: `Allow`, `Queue`, `Replace`.

### 7.3 ViewModel lifecycle

```
Construct (DI)
  → Accept(args) / Accept(query)    when IAcceptNavArgs / IAcceptNavQuery
  → InitializeAsync(token)          once
  → OnNavigatedToAsync(token)
  → OnAppearingAsync(token)
  → OnDisappearingAsync(token)
  → OnNavigatedFromAsync(token)
  → Dispose
```

Core: `ViewModelCancellationToken` is created in the constructor and cancelled on dispose. The token stays readable after dispose. `UseMvvmExpress` accepts `MvvmExpressOptions.CancelOperationsOnDisappear`; the current `ViewModelLifecycleBehavior` calls `OnDisappearingAsync` and does not yet cancel the token on disappear.

### 7.4 Unified async state

`ViewModelStatus`: Idle, Loading, Refreshing, Saving, Success, Empty, Error, Offline, Unauthorized, Cancelled.

`AsyncState<T>` holds Status, Data, Error, Exception, Timestamp, and derived flags. Transitions go through `IStateMachine<ViewModelStatus>` when the optional machine is enabled.

### 7.5 Outcome

`Outcome` / `Outcome<T>` is the library result type (success / failure with code, message, exception, validation, metadata). Named `Outcome` to stay out of the crowded `Result<T>` ecosystem.

### 7.6 Operation pipeline

Designed: `IOperationExecutor.RunAsync(...)` as the single entry that commands, `ExecuteBusyAsync`, pagination, and search share (busy + cancellation + timeout + retry + error sink + logging + telemetry + `Outcome`). **Not shipped in 0.3.0** — commands, `AsyncState`, and `SearchQuery` each own their own slice of that pipeline today.

This remains the primary differentiator. It is not a Polly clone and not a ReactiveUI `ReactiveCommand` clone.

### 7.7 Scopes

```
Application scope     (singleton services, app ViewModels)
  Window scope        (IWindowContext — multi-window)
    Navigation scope  (stack / Shell section)
      Page scope      (one PageViewModel)
        Child scope   (composed child ViewModels)
```

Scopes are `IServiceScope` instances owned by `IViewModelScopeFactory`. Navigating away disposes the page scope unless the page stays on the stack (then the scope is retained until pop).

## 8. Navigation

`INavigator` is host-agnostic.

| Host | Package type | When to use |
| --- | --- | --- |
| `MauiShellNavigator` | Shell routes, query, deep links | Apps already on Shell |
| `MauiPageNavigator` / `IPageNavigator` | `INavigation` / `NavigationPage` | Apps that do not want Shell |

Prism.Maui (current public docs) does **not** support Shell navigation and uses URI + dictionary parameters. MVVMExpress supports both hosts and prefers `NavigateToAsync<TViewModel, TArgs>(TArgs args)` with `record` parameters. Dictionary / URI interop is `NavigateToAsync(route, query)` + `IAcceptNavQuery`.

Guards: `CanNavigateAwayAsync`. `[RequiresAuth]` / `[RequiresRole]` and `[PersistState]` ship in `0.5.0-preview`.

Stack (shipped on `INavigator`): `Current` (`Type?`), `Stack`, `ModalStack`, `CanGoBack`, `History`, `GoBackAsync`, `PopToRootAsync`, `ReplaceAsync`, `ResetAsync`. Multi-window: `IWindowContext`, `WindowNavigatorRegistry`, `MauiWindowContext`.

## 9. What is MAUI-specific vs platform-independent

| Independent (Core / Validation / Pagination / Reactive / Testing) | MAUI-specific (Host / Navigation / Dialogs) |
| --- | --- |
| ObservableModel, commands, state, outcome | Lifecycle behaviors |
| Messaging, busy, retry, timeout | `IMainThread` MAUI implementation |
| Forms, dirty, undo, collections | Shell / `INavigation` hosts |
| Pagination, search (logic) | Dialog / toast implementations |
| Validation engine | View/ViewModel locator attached to `Page` |
| Abstractions for connectivity, cache, files, permissions | Platform pickers, permissions, clipboard adapters |
| Source generator attributes | Window / app lifecycle bridge |

`net10.0` (no OS TFM) is the shared / test surface. Host APIs that need a window throw `FeatureNotSupportedException` on that TFM.

**Primary support:** Android (`net10.0-android`, API 21+) and iOS (`net10.0-ios`, iOS 15+), matching the catalog.  
**Compile targets:** Mac Catalyst and Windows (`net10.0-windows10.0.19041.0`, Windows-only build) are included because the master prompt requires them. They are not claimed as catalog-primary until samples and tests exist.

## 10. CommunityToolkit.Mvvm — conflicts and integration

Studied: CommunityToolkit.Mvvm 8.4 (partial-property `[ObservableProperty]`, `[RelayCommand]`, `ObservableObject`, `ObservableValidator`, `ObservableRecipient`, `IMessenger`, AOT analyzers).

| Conflict | Resolution |
| --- | --- |
| Same type names if we reimplement `ObservableObject` / `RelayCommand` / `[ObservableProperty]` | Different names (`ObservableModel`, `AsyncModelCommand`, `[Notify]`) |
| Dual source generators on one type | Document: one generator per type. Compatibility package can wrap CT objects, not decorate them twice |
| `IMessenger` vs our hub | `IMessageHub`; adapter implements both directions in Compatibility package |
| Apps already inherit `ObservableObject` | `PageViewModel` can wrap an existing CT object, or inherit `ObservableModel`. No forced base-class swap in Phase 1 |
| We must not take a Core PackageReference on CommunityToolkit.Mvvm | Compatibility is optional |

MVVMExpress does **not** reimplement CommunityToolkit feature-for-feature. Properties and basic commands are implemented because Core must work standalone. Advanced CT-only analyzers are not cloned.

## 11. Prism.Maui — conflicts and integration

Studied: Prism.Maui 9 page navigation (`INavigationService`, URI segments, `INavigationParameters` dictionary, `IInitialize` / `INavigationAware`, `IDialogService`, DryIoc/Unity). Public docs: **Shell is not supported**; regions are Prism-specific.

| Conflict | Resolution |
| --- | --- |
| `INavigationService` name | `INavigator` |
| Dictionary-first parameters | Typed records first; `IDictionary<string, object>` overload for interop |
| Container replacement (DryIoc) | Microsoft.Extensions.DependencyInjection only |
| `IDialogAware` page-as-dialog | `IDialogs` uses MAUI display APIs + optional custom pages; not Prism dialog containers |
| Regions | Not in v1. Child ViewModel composition replaces the common use case |
| Migration | Docs map `NavigateAsync("X")` → `NavigateToAsync<XViewModel>()` |

## 12. ReactiveUI — conflicts and integration

Studied: `ReactiveObject`, `ReactiveCommand`, `WhenAnyValue`, `ObservableAsPropertyHelper`, `WhenActivated`, `IScreen` / `RoutingState`, `ReactiveUI.SourceGenerators`.

| Conflict | Resolution |
| --- | --- |
| System.Reactive required for core RxUI | Core has **no** Rx dependency. Reactive package may reference it |
| `WhenActivated` leak discipline | Lifecycle + `IDisposable` bag on `ViewModel`; GC tests prove it |
| ViewModel-first routing (`IScreen`) | Optional later; v1 navigation is page/Shell host based |
| Dual command systems | `AsyncModelCommand` is the default. Reactive package adds `ReactiveModelCommand` that can wrap the pipeline |

Derived state in Core without Rx: `Computed<T>` / `IDependentProperty` updated from `[NotifyAlso]`. Full `CombineLatest` lives in the Reactive package.

## 13. Features that need source generators

| Attribute | Generates | Phase |
| --- | --- | --- |
| `[Notify]` | Property + changing/changed + dependents | 4 |
| `[ModelCommand]` / `[AsyncModelCommand]` | Command property + CanExecute hookup | 4 |
| `[RegisterViewModel]` / `[RegisterView]` | `IServiceCollection` extension | 4 |
| `[Route]` | Route table for Shell / navigator | 4 |
| `[NotifyAlso]` / `[DependsOn]` | Dependent notifications | 4 |
| `[PersistState]` | Save/restore members | 4 |
| `[RequiresAuth]` / `[RequiresRole]` | Guard metadata | 4 |

Until generators ship, all of the above is handwritten. Attributes may exist in Core as no-op markers so samples can adopt them early.

## 14. AOT / trimming risks

| Risk | Mitigation |
| --- | --- |
| Convention scan (`*Page` / `*ViewModel`) | Generator registration is the supported AOT path. Reflection scan is opt-in and annotated with `DynamicallyAccessedMembers` / `RequiresUnreferencedCode` |
| `Activator.CreateInstance` for pages | Resolve from DI (`IServiceProvider.GetRequiredService<TPage>()`) |
| Dictionary navigation by string type name | Typed `NavigateToAsync<TViewModel>()` is the AOT path; string routes require a generated route table |
| DataAnnotations | Use source-generated validators where possible; document trim warnings on `ValidationContext` |
| Messaging by `typeof(TMessage)` | Closed generic `IMessageHub.Subscribe<T>` — no string topic required |
| JSON state restoration | `System.Text.Json` + `[JsonSerializable]` context supplied by the app |
| FluentValidation / Rx optional refs | Never referenced from Core; no trim graph leak |

## 15. Memory-leak risks

| Risk | Mitigation |
| --- | --- |
| Page → Behavior → ViewModel → Page | Behaviors unsubscribe on `Unloaded`. ViewModel does not hold `Page` |
| Strong messenger subscriptions | Default `IMessageHub` is weak. Strong subscribe is explicit and `IDisposable` |
| Command `CanExecuteChanged` + property events | Commands hold weak refs to the model or dispose with the ViewModel |
| Navigation stack retaining popped VMs | Page scope dispose on pop |
| Child ViewModels | Parent dispose walks children |
| Static `Application.Current` handlers | Host registers `IDisposable` with the MAUI app lifetime |
| Reactive subscriptions | `ViewModel.Trash` (`CompositeDisposable`-like) cleared on deactivate |
| EventAggregator | Same as messenger: weak by default |

**Required tests:** `WeakReference` + `GC.Collect` for ViewModel, command, messenger, behavior, and navigator cases (Phase 1 for Core; Phase 2 for navigation).

## 16. Threading and concurrency

| Surface | Guarantee |
| --- | --- |
| `ObservableModel.SetProperty` | Not thread-safe. Call from UI thread or marshal via `IMainThread` |
| `AsyncState<T>.Set*` | Thread-safe via compare-exchange; `PropertyChanged` marshalled if dispatcher present |
| `AsyncModelCommand` | Execution serialized or concurrent per `ConcurrencyMode`. `IsRunning` is atomic |
| Cancellation | Cooperative; dispose/cancel is thread-safe |
| `INavigator` | Serialized per window (one navigation at a time). Concurrent calls queue or fail with `Outcome` |
| `ObservableRangeCollection<T>` | Same-thread as the bound view unless `IMainThread` marshal option is on |
| Messaging | Handlers invoked on the publishing thread unless `IMainThread` is requested |

Library code uses `ConfigureAwait(false)` **except** where the next step must run on the captured UI context (navigation host, dialog host, property notification marshal).

## 17. Configuration

```csharp
builder.UseMvvmExpress(options =>
{
    options.CancelOperationsOnDisappear = true; // option exists; lifecycle behavior does not cancel the token yet
});
```

Shipped `MvvmExpressOptions` has only `CancelOperationsOnDisappear`. Designed flags (`EnableNavigation`, `EnableLifecycle`, `EnableAutoRegistration`, `EnableDiagnostics`, `EnableReactive`) are not on the type yet — register Navigation / Dialogs implementations in the app.

**Minimal mode:** `UseMvvmExpress()` → Core + lifecycle + dispatcher + ViewModel resolve.  
**Enterprise mode:** add Navigation, Dialogs, Validation, Pagination, and app-supplied adapters for cache / offline / auth / flags.

## 18. Telemetry and logging

- `ILogger<T>` is optional. Null logger is the default.
- `IMvvmExpressTelemetry` hooks: command duration, navigation duration, VM create/dispose, errors, state transitions.
- No Application Insights, OpenTelemetry exporter, or Observability package reference.
- Never log passwords, tokens, or raw PII. Structured properties use allow-listed names.

## 19. Security

- ViewModels do not store secrets. Tokens belong in SecureSession / SecureStoragePlus.
- `[PersistState]` refuses properties marked `[Sensitive]` / `[DoNotPersist]`.
- Authorization attributes only consult `IAuthState` / `IAuthorizationPolicy` supplied by the app.
- File/media abstractions never default to world-readable shared storage.

## 20. Repository layout

```
MVVMExpress/
├── ARCHITECTURE.md          ← this file
├── API-DESIGN.md
├── DESIGN.md
├── DESIGN-PLAN.md
├── ROADMAP.md
├── FEATURE-MATRIX.md
├── MEMORY-AND-PERFORMANCE.md
├── README.md
├── LICENSE
├── src/                     packages
├── tests/
├── samples/                 flyout host + shared net10.0 ViewModels
├── benchmarks/              host-process timings
└── docs/
```

This folder is its own git repository and MauiEssentials submodule (`Plugin.Maui.MVVMExpress`), matching every other plugin.

## 21. How to read this document

This file is the architecture contract. **0.5.0-preview implements** Core through Reactive, source generators, persist/auth attributes, CommunityToolkit compatibility, and Phase 5 productization. Remaining 1.0.0 work is design-review sign-off; accepted scope is in [docs/known-limitations.md](docs/known-limitations.md). Shipping versus designed is tracked in [FEATURE-MATRIX.md](FEATURE-MATRIX.md). See [ROADMAP.md](ROADMAP.md).
