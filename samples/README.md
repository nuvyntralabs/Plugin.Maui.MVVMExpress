# MVVMExpress samples

ViewModels live in [`Plugin.Maui.MVVMExpress.Samples.Shared`](Plugin.Maui.MVVMExpress.Samples.Shared/) (`net10.0`, no MAUI). They are covered by [`tests/Plugin.Maui.MVVMExpress.Samples.Tests`](../tests/Plugin.Maui.MVVMExpress.Samples.Tests/).

The MAUI host [`Plugin.Maui.MVVMExpress.Sample`](Plugin.Maui.MVVMExpress.Sample/) is a flyout app with one XAML page per scenario. `MauiProgram` calls `UseMvvmExpress()`, then `AddMvvmExpressSamples()`, then replaces `INavigator` with `GuardedNavigator` around `MauiShellNavigator` and `IPageNavigator` with `MauiPageNavigator`. Home → products switches the CRUD flyout; Home → details (typed or URI query) and login → secure home push real Shell pages. **Page stack** pushes onto `INavigation` and shows a `MauiNotifier` toast.

Each sample integrates **library** types. In-memory implementations (`InMemoryNavigator`, `MemoryCache`, `InMemoryConnectivityProbe`, `InMemoryAuthState`) are for the demo host. Production apps should adapt sibling plugins instead of shipping those in-memory types.

| Sample | ViewModels | Library integration |
| --- | --- | --- |
| [Basic](Plugin.Maui.MVVMExpress.Samples.Shared/Basic/) | `CounterViewModel` | `ViewModel`, `SetProperty`, `NotifyDependsOn`, `ModelCommand` |
| [CRUD](Plugin.Maui.MVVMExpress.Samples.Shared/Crud/) | `ProductListViewModel`, `ProductEditViewModel` | `FormViewModel` dirty / undo / redo, `AsyncState<T>`, `IValidator` / DataAnnotations |
| [Navigation](Plugin.Maui.MVVMExpress.Samples.Shared/Navigation/) | `HomeViewModel`, `ProductDetailsViewModel`, `PageStackViewModel`, `PageStackItemViewModel` | `PageViewModel`, `INavigator` / `IPageNavigator`, typed + URI query, stack APIs, `INotifier` toast, dirty guard |
| [Auth](Plugin.Maui.MVVMExpress.Samples.Shared/Auth/) | `LoginViewModel`, `SecureHomeViewModel` | `IAuthState`, `GuardedNavigator` — adapt [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) in production |
| [Offline](Plugin.Maui.MVVMExpress.Samples.Shared/Offline/) | `OfflineCatalogViewModel` | `ICachedFetcher` + `FetchPolicy`, `IConnectivityProbe` — adapt [Plugin.Maui.ApiCache](https://www.nuget.org/packages/Plugin.Maui.ApiCache) / [Plugin.Maui.OfflineSync](https://www.nuget.org/packages/Plugin.Maui.OfflineSync) |
| [Pagination](Plugin.Maui.MVVMExpress.Samples.Shared/Pagination/) | `PagedProductViewModel` | `DelegatePagedCollection<T>` load-more + refresh |
| [Reactive](Plugin.Maui.MVVMExpress.Samples.Shared/Reactive/) | `SearchViewModel` | `SearchQuery` debounce + `PropertyObservable.CombineLatest` `FullName` |
| [Enterprise](Plugin.Maui.MVVMExpress.Samples.Shared/Enterprise/) | `EnterpriseShellViewModel`, `CatalogStatusViewModel` | Child composition, `IFeatureSwitch`, hub, busy, probe, auth gate — adapt [Plugin.Maui.NetworkMonitor](https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor) / [Plugin.Maui.FeatureFlags](https://www.nuget.org/packages/Plugin.Maui.FeatureFlags) |

```bash
dotnet test tests/Plugin.Maui.MVVMExpress.Samples.Tests
dotnet build samples/Plugin.Maui.MVVMExpress.Sample/Plugin.Maui.MVVMExpress.Sample.csproj
```

Those NuGet packages are Niladri Padhy / MauiEssentials / Nuvyntra Labs work. Usual alternatives: MAUI `SecureStorage`, raw `HttpClient`, CommunityToolkit.Maui, Polly.
