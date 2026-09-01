# MVVMExpress Feature Matrix

Comparison of **Plugin.Maui.MVVMExpress** against publicly documented capabilities of:

- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) 8.4 (source generators, `ObservableObject`, `RelayCommand`, `IMessenger`, `ObservableValidator`)
- [Prism.Maui](https://prismlibrary.github.io/docs/maui/navigation/index.html) 9 (page + region navigation, `INavigationService`, `IDialogService`; **Shell not supported**)
- [ReactiveUI](https://www.reactiveui.net/documentation/getting-started/installation/maui/) + [ReactiveUI.SourceGenerators](https://github.com/reactiveui/ReactiveUI.SourceGenerators) (`ReactiveObject`, `ReactiveCommand`, `WhenAnyValue`, `WhenActivated`, `IScreen`)

**Honesty rule:** The [README](README.md) comparison is the **designed product** (Yes = in the architecture). This file tracks **shipping**. `Yes` here means types exist **and** tests exist. `Designed (Pn)` means specified for phase n, not coded yet. This table does not claim superiority. Device RSS numbers are budgets until Phase 5 samples.

Last validated: 2026-09-01 against the public docs and repos linked above.

## Legend

| Mark | Meaning |
| --- | --- |
| Yes | Documented, first-class |
| Partial | Exists with limits or via a different package / pattern |
| Ext | Typical community / extra package, not the core product |
| No | Not a documented product feature |
| Designed (Pn) | Specified for phase n; not shipped |
| Yes | Shipped in this repo with tests |

## Core MVVM

| Feature | MVVMExpress | CommunityToolkit.Mvvm | Prism.Maui | ReactiveUI |
| --- | --- | --- | --- | --- |
| Observable properties | Yes | Yes | Yes (`BindableBase`) | Yes |
| `INotifyPropertyChanging` | Yes | Yes | Partial | Yes |
| Dependent / computed properties | Yes (`NotifyDependsOn`; generators P4) | Partial (`NotifyPropertyChangedFor`) | No | Yes (OAPH) |
| Source generators for properties | Designed (P4) | Yes (`[ObservableProperty]`, partial properties in 8.4) | No | Yes (`[Reactive]`) |
| Manual INPC without generators | Yes | Yes | Yes | Yes |
| Sync commands | Yes | Yes (`RelayCommand`) | Yes (`DelegateCommand`) | Yes |
| Async commands | Yes | Yes (`AsyncRelayCommand`) | Partial | Yes (`ReactiveCommand`) |
| Command `CanExecute` refresh | Yes | Yes | Yes | Yes (observable) |
| Command cancellation | Yes | Yes | Partial | Yes |
| Command timeout / retry / debounce / throttle | Partial (timeout + retry shipped; debounce via `SearchQuery`) | No | No | Ext (Rx operators) |
| Concurrency modes (cancel previous, queue) | Partial (prevent + cancel-previous; queue Designed P1) | Partial (concurrent flag) | No | Partial |
| Composite commands | Designed (P1) | No | Yes | Ext |
| `ObservableValidator` / DataAnnotations | Yes (`IValidator` / `DataAnnotationsValidator`) | Yes | Ext | Ext |
| Messenger | Yes (weak default) | Yes (`IMessenger`) | Yes (EventAggregator) | Yes (`MessageBus`) |
| ViewModel base + lifecycle | Yes | No | Yes (`INavigationAware`, `IInitialize`) | Yes (`WhenActivated`) |
| Memory-leak GC tests | Yes | Partial | Partial | Partial |
| Small / mid / large batch collections | Yes | App code | App code | App / Rx |

## Navigation and app structure

| Feature | MVVMExpress | CommunityToolkit.Mvvm | Prism.Maui | ReactiveUI |
| --- | --- | --- | --- | --- |
| ViewModel navigation service | Yes (`INavigator`, `InMemoryNavigator`) | No | Yes | Yes (`IScreen` / `RoutingState`) |
| Shell navigation host | Yes (`MauiShellNavigator` routes + URI stack) | No | No (docs: Shell not supported) | Partial |
| Page / `INavigation` host | Yes (`MauiPageNavigator` / `IPageNavigator`) | No | Yes (primary) | Partial |
| Typed navigation parameters | Yes (`IAcceptNavArgs<T>`, `record` args) | No | No (dictionary / URI query) | Partial |
| Dictionary / URI parameters | Yes (`NavigateToAsync(route, query)`, `IAcceptNavQuery`) | No | Yes | Partial |
| Navigation guards / cancel | Yes (`CanNavigateAwayAsync`, dirty `InMemoryNavigator` guard) | No | Partial (`IConfirmNavigation`) | Partial |
| Navigation stack API | Yes (`Stack`, `ModalStack`, `CanGoBack`, `PopToRoot`, `Replace`, `Reset`) | No | Yes | Yes |
| Regions | No (v1) | No | Yes | No |
| Dialogs from ViewModel | Yes (`IDialogs`, `NullDialogs`, `MauiDialogs`) | No (use MAUI / Toolkit) | Yes | Ext |
| Toast / snackbar abstraction | Yes (`INotifier`) | No | Ext | Ext |
| DI via `Microsoft.Extensions.DependencyInjection` | Yes (`AddMvvmExpress`, `UseMvvmExpress`) | App-level only | Partial (Prism containers; MS.DI adapters exist) | Partial (Splat / `RxAppBuilder`) |
| Convention View/VM registration | Designed (P4) | No | Yes | Partial |
| ViewModel locator (optional) | Designed (P1) | No | Yes | Yes |
| Multi-window navigation root | Yes (`IWindowContext`, `WindowNavigatorRegistry`) | No | Partial (`PrismWindow`) | Partial |

## State, data, and MAUI productivity

| Feature | MVVMExpress | CommunityToolkit.Mvvm | Prism.Maui | ReactiveUI |
| --- | --- | --- | --- | --- |
| Unified `AsyncState<T>` (Idle/Loading/Empty/Error/…) | Yes | No | No | Ext |
| Explicit state machine | Designed (P1) | No | No | Ext |
| Result / `Outcome` type | Yes | No | `INavigationResult` only | No |
| Pagination / infinite scroll | Yes (`PagedCollection<T>`, `DelegatePagedCollection<T>`) | No | Ext | Ext |
| Pull-to-refresh abstraction | Yes (`PagedCollection.RefreshAsync`) | No | No | Ext |
| Search debounce + cancel | Yes (`SearchQuery`) | No | No | Yes (Rx) |
| Reactive derived state | Designed (P3) | No | No | Yes |
| Requires System.Reactive for core | No | No | No | Yes |
| Offline / cache abstractions | Yes (`ICache` / `MemoryCache`; production: ApiCache / OfflineSync) | No | No | Ext |
| Form + dirty + submit | Designed (P3) | Partial (validator only) | Ext | Ext |
| Undo / redo | Designed (P3) | No | No | Ext |
| Connectivity abstraction | Yes (`IConnectivityProbe`) | No | No | Ext |
| Lifecycle-aware cancellation | Yes (dispose cancels token) | No | Partial | Partial (`WhenActivated`) |
| State restoration (`[PersistState]`) | Designed (P4) | No | Partial | Partial |
| Deep linking | Designed (P4) | No | Yes (URI) | Yes |
| Auth navigation guards | Yes (`IAuthState`, `GuardedNavigator`) | No | Ext | Ext |
| Feature-flag abstraction | Designed (P3) | No | No | No |
| Testing leak/scale helpers | Yes (`LeakProbe`, `ScaleProfile`) | Partial | Yes | Yes |
| Testing fakes package | Yes (`FakeDialogs`, `FakeNavigator`) | Partial | Yes | Yes |
| MAUI page lifecycle behaviors | Yes (`ViewModelLifecycleBehavior`) | No | Yes | Yes (`ReactiveContentPage`) |
| AOT / trim-friendly registration | Designed (P4) | Yes (analyzers in 8.4) | Partial | Partial |

## Differentiating row (design intent, not a claim of current quality)

| Idea | MVVMExpress | Others |
| --- | --- | --- |
| Single operation pipeline (busy + cancel + timeout + retry + concurrency + `Outcome`) | Designed backbone | Split across app code, Polly, Rx, or not present |
| `AsyncState<T>` as bindable UI status | Yes | Usually hand-rolled booleans |
| Shell **or** page host without requiring either | Yes (P2) | Prism: page only; CT: none; RxUI: router |
| Typed `record` navigation args as the default | Yes (`IAcceptNavArgs<T>`) | Prism/RxUI are string/URI/dictionary-first |
| ViewModel scopes (app / window / nav / page / child) | Designed (P3) | Prism container + regions; not the same model |
| Compose MauiEssentials plugins via adapters | Designed | Out of scope for CT/Prism/RxUI |

## MauiEssentials plugins vs MVVMExpress

MVVMExpress does **not** replace these packages. Use them when the requirement matches; adapt them into MVVMExpress interfaces.

| Requirement | Package | NuGet |
| --- | --- | --- |
| Captive portal / real internet | Plugin.Maui.NetworkMonitor | https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor |
| HTTP GET cache | Plugin.Maui.ApiCache | https://www.nuget.org/packages/Plugin.Maui.ApiCache |
| Offline sync | Plugin.Maui.OfflineSync | https://www.nuget.org/packages/Plugin.Maui.OfflineSync |
| Fluent / `Validation.For` forms | Plugin.Maui.FormValidation | https://www.nuget.org/packages/Plugin.Maui.FormValidation |
| Feature flags | Plugin.Maui.FeatureFlags | https://www.nuget.org/packages/Plugin.Maui.FeatureFlags |
| App Links / Universal Links | Plugin.Maui.DeepLinks | https://www.nuget.org/packages/Plugin.Maui.DeepLinks |
| Permission UX | Plugin.Maui.PermissionFlow | https://www.nuget.org/packages/Plugin.Maui.PermissionFlow |
| Tokens / session | Plugin.Maui.SecureSession | https://www.nuget.org/packages/Plugin.Maui.SecureSession |

GitHub hub: https://github.com/nuvyntralabs/MauiEssentials

## Maintenance

When a phase ships, change **Designed (Pn)** to **Yes** only if tests exist. If a competitor ships a new first-class feature, update this file in the same PR that notices it.
