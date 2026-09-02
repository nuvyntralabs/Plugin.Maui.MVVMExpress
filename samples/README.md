# MVVMExpress samples

ViewModels live in [`Plugin.Maui.MVVMExpress.Samples.Shared`](Plugin.Maui.MVVMExpress.Samples.Shared/) (`net10.0`, no MAUI). They are covered by [`tests/Plugin.Maui.MVVMExpress.Samples.Tests`](../tests/Plugin.Maui.MVVMExpress.Samples.Tests/).

**Playground (15-minute click tour):** [`Playground`](Playground/) — command, navigation, dialog, form, auth, list. `dotnet run --project samples/Playground/Plugin.Maui.MVVMExpress.Playground.csproj -f net10.0-maccatalyst`.

**First-run login app:** [`Plugin.Maui.MVVMExpress.AuthApp`](Plugin.Maui.MVVMExpress.AuthApp/) — login / register / forgot / guarded home (`UseAuth<AuthLoginViewModel>()`, `ResetAsync` replace-root). Demo: `demo@mvvmexpress.dev` / `secret`.

**Chat host (ViewModels only):** [`ChatHost`](Plugin.Maui.MVVMExpress.Samples.Shared/ChatHost/) — `SectionHostViewModel` + `SnapshotCollection` + Entry `SearchQuery`. Cookbook: [docs/chat-host.md](../docs/chat-host.md). Do not treat this as a `PagedCollection` + `CollectionView` sample.

The MAUI host [`Plugin.Maui.MVVMExpress.Sample`](Plugin.Maui.MVVMExpress.Sample/) is a flyout app with one XAML page per scenario. `MauiProgram` calls `UseMvvmExpress()`, then `AddMvvmExpressSamples()`, then replaces `INavigator` with `GuardedNavigator` around `MauiShellNavigator` and `IPageNavigator` with `MauiPageNavigator`. Home → products switches the CRUD flyout; Home → details (typed or URI query) and login → secure home push real Shell pages. **Page stack** pushes onto `INavigation` and shows a `MauiNotifier` toast.

Each sample integrates **library** types. In-memory implementations (`InMemoryNavigator`, `MemoryCache`, `InMemoryConnectivityProbe`, `InMemoryAuthState`) are for the demo host. Production apps should adapt sibling plugins instead of shipping those in-memory types.

| Sample | ViewModels | Library integration |
| --- | --- | --- |
| [Basic](Plugin.Maui.MVVMExpress.Samples.Shared/Basic/) | `CounterViewModel` | `ViewModel`, `SetProperty`, `NotifyDependsOn`, `ModelCommand` |
| [CRUD](Plugin.Maui.MVVMExpress.Samples.Shared/Crud/) | `ProductListViewModel`, `ProductEditViewModel` | `FormViewModel` dirty / undo / redo, `AsyncState<T>`, `IValidator` / DataAnnotations |
| [Navigation](Plugin.Maui.MVVMExpress.Samples.Shared/Navigation/) | `HomeViewModel`, `ProductDetailsViewModel`, `PageStackViewModel`, `PageStackItemViewModel`, `ScopedCatalogFlowViewModel` | `PageViewModel`, `INavigator` / `IPageNavigator`, typed + URI query, stack APIs, `INotifier` toast, dirty guard, page-scope push/pop (`IViewModelScopeFactory`) |
| [Auth](Plugin.Maui.MVVMExpress.Samples.Shared/Auth/) | `LoginViewModel`, `SecureHomeViewModel` | `IAuthState`, `GuardedNavigator` — adapt [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) in production |
| [Offline](Plugin.Maui.MVVMExpress.Samples.Shared/Offline/) | `OfflineCatalogViewModel` | `ICachedFetcher` + `FetchPolicy`, `IConnectivityProbe` — adapt [Plugin.Maui.ApiCache](https://www.nuget.org/packages/Plugin.Maui.ApiCache) / [Plugin.Maui.OfflineSync](https://www.nuget.org/packages/Plugin.Maui.OfflineSync) |
| [Pagination](Plugin.Maui.MVVMExpress.Samples.Shared/Pagination/) | `PagedProductViewModel` | `DelegatePagedCollection<T>` load-more + refresh (not for a live chat inbox) |
| [Chat host](Plugin.Maui.MVVMExpress.Samples.Shared/ChatHost/) | `ChatHostViewModel`, `ChatInboxViewModel` | `SectionHostViewModel`, `SnapshotCollection<T>`, `SearchQuery.CommittedText`, `CoalescingDispatcher` |
| [Reactive](Plugin.Maui.MVVMExpress.Samples.Shared/Reactive/) | `SearchViewModel` | `SearchQuery` debounce + `PropertyObservable.CombineLatest` `FullName` |
| [Enterprise](Plugin.Maui.MVVMExpress.Samples.Shared/Enterprise/) | `EnterpriseShellViewModel`, `CatalogStatusViewModel` | Child composition, `IFeatureSwitch`, hub, busy, probe, auth gate — adapt [Plugin.Maui.NetworkMonitor](https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor) / [Plugin.Maui.FeatureFlags](https://www.nuget.org/packages/Plugin.Maui.FeatureFlags) |
| [Generated](Plugin.Maui.MVVMExpress.Samples.Shared/Generated/) | `GeneratedCatalogViewModel` | `[Notify]`, `[ModelCommand]`, `[PersistState]`, `[RegisterViewModel]`, `[Route]`, `[RequiresAuth]` — AOT `AddGeneratedViewModels()` |
| Deep links | `DeepLinkRouteMap` | URI → `INavigator` route/query — compose [Plugin.Maui.DeepLinks](https://www.nuget.org/packages/Plugin.Maui.DeepLinks) in production |

```bash
dotnet test tests/Plugin.Maui.MVVMExpress.Samples.Tests
dotnet build samples/Plugin.Maui.MVVMExpress.Sample/Plugin.Maui.MVVMExpress.Sample.csproj
```

Those NuGet packages are Niladri Padhy / MauiEssentials / Nuvyntra Labs work. Usual alternatives: MAUI `SecureStorage`, raw `HttpClient`, CommunityToolkit.Maui, Polly.
