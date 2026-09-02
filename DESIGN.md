# MVVMExpress Design Document

This is the product design for **Plugin.Maui.MVVMExpress**. It explains how a developer uses the framework and why each subsystem exists. Public type signatures live in [API-DESIGN.md](API-DESIGN.md). Package graph and risks live in [ARCHITECTURE.md](ARCHITECTURE.md).

## 1. Audience

- MAUI developers who outgrew CommunityToolkit.Mvvm (need navigation, lifecycle, async state).
- Teams considering Prism.Maui who want Microsoft.Extensions.DependencyInjection, Shell *or* page navigation, and typed parameters.
- Teams considering ReactiveUI who want derived state without taking System.Reactive in every project.
- Teams already using MauiEssentials plugins who need an application shell, not another connectivity or sync engine.

## 2. Non-goals (v1)

- Replacing .NET MAUI Shell, `Connectivity`, `Geolocation`, `SecureStorage`, or `Permissions` when those APIs are enough.
- Shipping a DI container (DryIoc, Autofac).
- Shipping a SIP stack, FCM registrar, SQLite sync engine, or remote feature-flag service.
- Treating multi-window desktop or sibling Android/iOS adapters (KeyboardManager, DeepLinks, SecureSession) as first-class on Mac Catalyst / Windows.
- Forking CommunityToolkit.Mvvm, Prism, or ReactiveUI.
- Implementing every master-prompt feature in the first commit.

## 3. Developer experience

### 3.1 Minimal app

```csharp
builder
    .UseMauiApp<App>()
    .UseMvvmExpress();

builder.Services.AddTransient<CounterViewModel>();
```

```csharp
public sealed class CounterViewModel : PageViewModel
{
    private int _count;

    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }

    public ModelCommand IncrementCommand { get; }

    public CounterViewModel()
    {
        IncrementCommand = new ModelCommand(() => Count++);
    }
}
```

No Shell, no generators, no Rx, no navigation package.

### 3.2 Production page

```csharp
public sealed class ProductListViewModel : PageViewModel
{
    public AsyncState<IReadOnlyList<Product>> Products { get; } = new();

    public AsyncModelCommand RefreshCommand { get; }

    public ProductListViewModel(ICatalog catalog, INavigator navigator)
    {
        RefreshCommand = new AsyncModelCommand(
            ct => Products.LoadAsync(() => catalog.ListAsync(ct), ct),
            new AsyncCommandOptions { CancelPrevious = true, Timeout = TimeSpan.FromSeconds(15) });
    }

    protected override Task OnAppearingAsync(CancellationToken cancellationToken)
        => RefreshCommand.ExecuteAsync(cancellationToken);
}
```

The ViewModel never calls `DisplayAlert`, `Shell.Current`, or `Connectivity.Current`.

### 3.3 Typed navigation

```csharp
public sealed record ProductDetailsArgs(int ProductId);

await Navigator.NavigateToAsync<ProductDetailsViewModel, ProductDetailsArgs>(
    new ProductDetailsArgs(productId));
```

The destination implements `IAcceptNavArgs<ProductDetailsArgs>` (or a generated partial). Dictionary parameters remain for deep-link and interop only.

## 4. Differentiating features

These are the features MVVMExpress is designed around. They are not present as a single API in CommunityToolkit.Mvvm, Prism.Maui, or ReactiveUI.

### 4.1 Operation pipeline

One executor behind commands, refresh, search, and “run this work”:

| Concern | How it is applied |
| --- | --- |
| Cancellation | ViewModel token + command token + caller token linked |
| Timeout | `AsyncCommandOptions.Timeout` |
| Retry | `IRetryPolicy` (fixed / exponential). Not Polly |
| Debounce / throttle | Options on command / search |
| Concurrency | Allow / Prevent / Queue / CancelPrevious / Replace |
| Progress | `IProgress<T>` + `IsRunning` |
| Busy | Nested `IBusyGate` |
| Errors | `IErrorSink` + `Outcome` |
| Logging / telemetry | Optional hooks |

### 4.2 Unified async state

`AsyncState<T>` is bindable UI state (Idle / Loading / Refreshing / Success / Empty / Error / Offline / Unauthorized / Cancelled), not a boolean. Templates bind to `IsEmpty`, `HasError`, `IsSuccess`.

### 4.3 Lifecycle-aware ViewModels

A behavior (or host hook) maps page events to ViewModel methods and optionally cancels work on disappear. The ViewModel does not subscribe to `Page.Appearing`.

### 4.4 Strongly typed navigation

Compile-time ViewModel + args. Hosts are swappable (Shell vs `INavigation`). Prism-style URI strings are optional, not the primary API.

### 4.5 ViewModel scopes

DI lifetime follows Application → Window → Navigation → Page → Child. Popping a page disposes its scope. This is the answer to “stale ViewModel” and multi-window, not `Application.Current.MainPage`.

### 4.6 Form engine

`FormViewModel` + `FormField<T>` combine validation, dirty, touched, submit, reset, and `CanNavigateAwayAsync`. Complements [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) (`Validation.For` XAML) rather than replacing it.

### 4.7 State restoration (Phase 4)

`[PersistState]` on selected properties. Sensitive members are excluded. Restoration is opt-in per ViewModel.

## 5. Composition with MauiEssentials

MVVMExpress is the MVVM shell. These plugins remain the capability implementations:

| Need | Do not reinvent | MVVMExpress surface |
| --- | --- | --- |
| Real internet / captive portal | [Plugin.Maui.NetworkMonitor](https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor) | `IConnectivityProbe` adapter |
| HTTP GET cache | [Plugin.Maui.ApiCache](https://www.nuget.org/packages/Plugin.Maui.ApiCache) | `ICache` adapter (CacheFirst / NetworkFirst / SWR) |
| Offline sync / conflicts | [Plugin.Maui.OfflineSync](https://www.nuget.org/packages/Plugin.Maui.OfflineSync) | repository policy hooks |
| HTTP retry / circuit breaker | [Plugin.Maui.ApiResilience](https://www.nuget.org/packages/Plugin.Maui.ApiResilience) | command retry is UI-level only |
| Failed named operations | [Plugin.Maui.RetryQueue](https://www.nuget.org/packages/Plugin.Maui.RetryQueue) | not a substitute |
| Form field XAML | [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) | Validation package + form engine |
| Feature flags | [Plugin.Maui.FeatureFlags](https://www.nuget.org/packages/Plugin.Maui.FeatureFlags) | `IFeatureSwitch` |
| App / Universal Links | [Plugin.Maui.DeepLinks](https://www.nuget.org/packages/Plugin.Maui.DeepLinks) | navigator deep-link map |
| Permission UX | [Plugin.Maui.PermissionFlow](https://www.nuget.org/packages/Plugin.Maui.PermissionFlow) | `IPermissionGate` |
| Tokens / 401 / biometrics | [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) | `IAuthState` |
| App lock after background | [Plugin.Maui.AppLock](https://www.nuget.org/packages/Plugin.Maui.AppLock) | not a biometric API |
| Clipboard / share | ClipboardPlus / SharePlus | `IClipboard`, `IShare` |
| Crash / telemetry suite | Diagnostics / Observability | `IMvvmExpressTelemetry` only |

If the app only needs GPS, print, NFC, or BLE, it should take those plugins and **not** take MVVMExpress.

## 6. View / ViewModel relationship

From the reference architecture (View owns VM, bindings both ways; navigator creates page+VM; VM owns model and talks to a store):

1. Host resolves `TPage` and `TViewModel` from the page scope.
2. Host sets `page.BindingContext = viewModel` (locator is optional; this is the default).
3. `ViewModelLifecycleBehavior` (or equivalent code-behind-free host hook) forwards Appearing / Disappearing / NavigatedTo / NavigatedFrom.
4. ViewModel updates `ObservableModel` / `AsyncState<T>`. Bindings refresh the view.
5. Store / repository is an app type. MVVMExpress does not own disk or HTTP.

The ViewModel knows nothing about a specific `ContentPage` subclass. The Model knows nothing about the ViewModel.

## 7. Error philosophy

```
Exception in command
    → OperationCanceledException → state Cancelled (not an error sink by default)
    → known domain error → Outcome.Failure(code, message)
    → unexpected → IErrorSink.Handle + optional rethrow in DEBUG
```

Empty `catch { }` is forbidden. `catch (Exception)` in pipeline code must call the sink or transform to `Outcome`.

User-facing text is produced by `IErrorSink` / app resources, not by concatenating exception messages into the UI.

## 8. Diagnostics

| Build | Default |
| --- | --- |
| Debug | Lifecycle and navigation traces via `ILogger` when `EnableDiagnostics = true` |
| Release | Diagnostics off. Binding-path analysis off |

Binding diagnostics (missing path, null BindingContext, resolve failure) are Host-only and trimmed out unless the diagnostics switch is compiled in.

## 9. Testing design

`Plugin.Maui.MVVMExpress.Testing` exists so a ViewModel test has:

- `FakeNavigator` (records `NavigateToAsync` calls)
- `FakeDialogs`
- `FakeMainThread` (runs inline)
- `FakeConnectivity`
- `FakeMessageHub`
- Lifecycle driver: `await vm.AppearAsync()` / `DisappearAsync()`
- `ScopedNavigator` (page-scope push/pop; pop disposes the ViewModel)

Core tests must run on `net10.0` with no MAUI runtime.

## 10. Versioning and stability

- 0.x — design and Phase 1–2; public API may change.
- 1.0 — Core + Host + Navigation + Dialogs + Validation + Testing are stable.
- SemVer after 1.0. Breaking changes require a major version and a migration note.

## 11. Open questions (historical — Phase 1/2 shipped in 0.3.0; remaining items feed Phase 3+)

1. ~~Confirm package prefix~~ — **MVVMExpress** / `Plugin.Maui.MVVMExpress` (accepted).
2. ~~Confirm Windows / Mac Catalyst stay as compile TFMs only~~ — **single-window host targets** (accepted). Multi-window stays later.
3. Confirm `Outcome` vs `Result` naming.
4. Confirm whether Host may auto-attach lifecycle without an explicit XAML behavior (preferred: yes, via `AddMvvmExpress`, behavior remains available).
5. Confirm CommunityToolkit compatibility is Phase 4, not Phase 1.
