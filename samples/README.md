# MVVMExpress samples

ViewModels live in [`Plugin.Maui.MVVMExpress.Samples.Shared`](Plugin.Maui.MVVMExpress.Samples.Shared/) (`net10.0`, no MAUI). They are covered by [`tests/Plugin.Maui.MVVMExpress.Samples.Tests`](../tests/Plugin.Maui.MVVMExpress.Samples.Tests/).

The MAUI host [`Plugin.Maui.MVVMExpress.Sample`](Plugin.Maui.MVVMExpress.Sample/) is a flyout app with one XAML page per scenario. `MauiProgram` calls `UseMvvmExpress()`, then `AddMvvmExpressSamples()`, then replaces `INavigator` with `GuardedNavigator` around `MauiShellNavigator`. Home → products switches the CRUD flyout; Home → details and login → secure home push real Shell pages.

Each sample integrates **library** types. In-memory implementations (`InMemoryNavigator`, `MemoryCache`, `InMemoryConnectivityProbe`, `InMemoryAuthState`) are for the demo host. Production apps should adapt sibling plugins instead of shipping those in-memory types.

| Sample | ViewModels | Library integration |
| --- | --- | --- |
| [Basic](Plugin.Maui.MVVMExpress.Samples.Shared/Basic/) | `CounterViewModel` | `ViewModel`, `SetProperty`, `NotifyDependsOn`, `ModelCommand` |
| [CRUD](Plugin.Maui.MVVMExpress.Samples.Shared/Crud/) | `ProductListViewModel`, `ProductEditViewModel` | `AsyncState<T>`, `AsyncModelCommand`, `ObservableRangeCollection`, `BusyGate`, `IErrorSink`, `MessageHub`, `IValidator` / DataAnnotations |
| [Navigation](Plugin.Maui.MVVMExpress.Samples.Shared/Navigation/) | `HomeViewModel`, `ProductDetailsViewModel` | `PageViewModel`, `INavigator`, `IAcceptNavArgs<ProductDetailsArgs>`, dirty `InMemoryNavigator` guard |
| [Auth](Plugin.Maui.MVVMExpress.Samples.Shared/Auth/) | `LoginViewModel`, `SecureHomeViewModel` | `IAuthState`, `GuardedNavigator` — adapt [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) in production |
| [Offline](Plugin.Maui.MVVMExpress.Samples.Shared/Offline/) | `OfflineCatalogViewModel` | `ICache` + cache-first catalog, `IConnectivityProbe` — adapt [Plugin.Maui.ApiCache](https://www.nuget.org/packages/Plugin.Maui.ApiCache) / [Plugin.Maui.OfflineSync](https://www.nuget.org/packages/Plugin.Maui.OfflineSync) |
| [Pagination](Plugin.Maui.MVVMExpress.Samples.Shared/Pagination/) | `PagedProductViewModel` | `DelegatePagedCollection<T>` load-more + refresh |
| [Reactive](Plugin.Maui.MVVMExpress.Samples.Shared/Reactive/) | `SearchViewModel` | `SearchQuery` debounce + `NotifyDependsOn` `FullName` |
| [Enterprise](Plugin.Maui.MVVMExpress.Samples.Shared/Enterprise/) | `EnterpriseShellViewModel` | `AddMvvmExpress` composition: hub, busy, probe, `IDialogs`, auth gate, `IMainThread` — adapt [Plugin.Maui.NetworkMonitor](https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor) |

```bash
dotnet test tests/Plugin.Maui.MVVMExpress.Samples.Tests
dotnet build samples/Plugin.Maui.MVVMExpress.Sample/Plugin.Maui.MVVMExpress.Sample.csproj
```

Those NuGet packages are Niladri Padhy / MauiEssentials / Nuvyntra Labs work. Usual alternatives: MAUI `SecureStorage`, raw `HttpClient`, CommunityToolkit.Maui, Polly.
