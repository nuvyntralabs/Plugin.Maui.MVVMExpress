# MVVMExpress Public API Design

Contract for **Plugin.Maui.MVVMExpress**. Default namespace root: `Plugin.Maui.MVVMExpress`. Shipped signatures here are the 1.0 contract. Breaking changes after 1.0.0 follow SemVer.

**How to read this file**

| Mark | Meaning |
| --- | --- |
| **Shipped (0.5.0-preview)** | Types exist in the packed packages **and** tests exist. Copy these signatures. |
| **Proposed** | Design intent only. Not in a nupkg. Do not implement against these names. |

Shipping versus designed is also tracked in [FEATURE-MATRIX.md](FEATURE-MATRIX.md). Phases live in [ROADMAP.md](ROADMAP.md). Architecture intent lives in [ARCHITECTURE.md](ARCHITECTURE.md).

**Shipped in 0.5.0-preview:** everything from released `0.4.0-preview` plus `[Notify]` / command / registration / persist / auth attributes, `MvvmExpressGeneratedRegistrations`, `IStateStore`, `INavigationAuthPolicy`, `IMvvmExpressDiagnostics` (Debug-only), and `CommunityToolkitMessageHub`.

---

## Shipped — 1. Hosting

```csharp
namespace Plugin.Maui.MVVMExpress.Hosting;

public static class MVVMExpressMauiAppBuilderExtensions
{
    public static MauiAppBuilder UseMvvmExpress(
        this MauiAppBuilder builder,
        Action<MvvmExpressOptions>? configure = null);
}

public static class MVVMExpressServiceCollectionExtensions
{
    public static IServiceCollection AddMvvmExpress(this IServiceCollection services);
}

public sealed class MvvmExpressOptions
{
    public bool CancelOperationsOnDisappear { get; set; }
    public bool EnableDiagnostics { get; set; }
}
```

`UseMvvmExpress` registers Core services, then replaces `IMainThread` with `MauiMainThread`. `AddMvvmExpress` has no options callback. `CancelOperationsOnDisappear` is stored; the current `ViewModelLifecycleBehavior` calls `OnDisappearingAsync` and does **not** yet cancel the token on disappear. Dispose is the guaranteed cancel path.

`AddMauiMvvm` is **not** used (avoids implying ownership of MAUI’s MVVM).

`AddMvvmExpress` defaults:

| Abstraction | Default | Typical app replacement |
| --- | --- | --- |
| `IMessageHub` | `MessageHub` | Keep |
| `IBusyGate` | `BusyGate` | Keep |
| `IErrorSink` | `NullErrorSink` | App logger sink |
| `ICache` | `MemoryCache` | Plugin.Maui.ApiCache adapter |
| `IConnectivityProbe` | `InMemoryConnectivityProbe` | Plugin.Maui.NetworkMonitor adapter |
| `IWindowContext` | `WindowContext.Default` | `MauiWindowContext.Current` |
| `IWindowNavigatorRegistry` | `WindowNavigatorRegistry` | Keep; register per window |
| `INavigator` / `IPageNavigator` | `InMemoryNavigator` | `MauiShellNavigator` / `MauiPageNavigator` |
| `IMainThread` | `ImmediateMainThread` | `MauiMainThread` (host) |
| `IDialogs` / `INotifier` | `NullDialogs` | `MauiDialogs` / `MauiNotifier` |

---

## Shipped — 2. Observable model and ViewModels

```csharp
namespace Plugin.Maui.MVVMExpress.ComponentModel;

public abstract class ObservableModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    protected bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null,
        IEqualityComparer<T>? comparer = null);

    protected bool SetProperty<T>(
        ref T field,
        T value,
        Action<T> onChanging,
        Action<T> onChanged,
        [CallerMemberName] string? propertyName = null);

    protected void Notify([CallerMemberName] string? propertyName = null);
    protected void NotifyChanging([CallerMemberName] string? propertyName = null);
    protected void NotifyDependsOn(string sourceProperty, params string[] dependents);
}

public interface IViewModel : IAsyncDisposable, IDisposable
{
    ViewModelStatus Status { get; }
    bool IsBusy { get; }
    CancellationToken ViewModelCancellationToken { get; }

    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task OnAppearingAsync(CancellationToken cancellationToken = default);
    Task OnDisappearingAsync(CancellationToken cancellationToken = default);
}

public interface INavigable
{
    Task OnNavigatedToAsync(CancellationToken cancellationToken = default);
    Task OnNavigatedFromAsync(CancellationToken cancellationToken = default);
    Task<bool> CanNavigateAwayAsync(CancellationToken cancellationToken = default);
}

public abstract class ViewModel : ObservableModel, IViewModel
{
    protected ViewModel(IErrorSink? errors = null, IBusyGate? busy = null);

    public ViewModelStatus Status { get; protected set; }
    public virtual bool IsBusy { get; }
    public CancellationToken ViewModelCancellationToken { get; }
    public bool IsDisposed { get; }

    protected Task<Outcome> ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    public virtual Task InitializeAsync(CancellationToken cancellationToken = default);
    public virtual Task OnAppearingAsync(CancellationToken cancellationToken = default);
    public virtual Task OnDisappearingAsync(CancellationToken cancellationToken = default);
}

public abstract class PageViewModel : ViewModel, INavigable
{
    protected PageViewModel(INavigator? navigator = null, IDialogs? dialogs = null);

    protected INavigator? Navigator { get; }
    protected IDialogs? Dialogs { get; }
}
```

There is no `NavigationContext` parameter on `INavigable`. Manual INPC is always valid. Generators ship in `0.5.0-preview`.

Lifecycle (hosts that apply args):

```
Construct (DI)
  → Accept(args) / Accept(query)    when IAcceptNavArgs / IAcceptNavQuery
  → InitializeAsync(token)          once
  → OnNavigatedToAsync(token)
  → OnAppearingAsync(token)
  → OnDisappearingAsync(token)
  → OnNavigatedFromAsync(token)
  → Dispose                         cancels ViewModelCancellationToken
```

The token stays readable after dispose (`IsCancellationRequested` is true).

---

## Shipped — 3. Commands

There is no `IModelCommand` / `IAsyncModelCommand` interface. Commands implement `ICommand` directly.

```csharp
namespace Plugin.Maui.MVVMExpress.Input;

public sealed class ModelCommand : ICommand
{
    public ModelCommand(Action execute, Func<bool>? canExecute = null);
    public event EventHandler? CanExecuteChanged; // weak; does not pin Button / page
    public void NotifyCanExecuteChanged();
}

public sealed class ModelCommand<T> : ICommand
{
    public ModelCommand(Action<T?> execute, Func<T?, bool>? canExecute = null);
    public void NotifyCanExecuteChanged();
}

public sealed class AsyncModelCommand : ObservableModel, ICommand
{
    public AsyncModelCommand(
        Func<CancellationToken, Task> execute,
        Func<bool>? canExecute = null,
        AsyncCommandOptions? options = null);

    public bool IsRunning { get; }
    public CommandExecutionState State { get; }
    public bool IsCancellationRequested { get; }

    public Task ExecuteAsync(CancellationToken cancellationToken = default);
    public void Cancel();
    public void NotifyCanExecuteChanged();
}

public sealed class AsyncModelCommand<T> : ObservableModel, ICommand { /* same + typed parameter */ }

public enum ConcurrencyMode { Prevent = 0, CancelPrevious = 1 }

public sealed class AsyncCommandOptions
{
    public ConcurrencyMode Concurrency { get; init; } = ConcurrencyMode.Prevent;
    public TimeSpan? Timeout { get; init; }
    public int RetryCount { get; init; }
    public TimeSpan RetryDelay { get; init; } = TimeSpan.FromMilliseconds(50);
}

public enum CommandExecutionState { Idle, Running, Completed, Failed, Cancelled }
```

`CompositeModelCommand` remains **proposed**. `Allow`, `Queue`, `Replace`, debounce, and throttle ship on `AsyncCommandOptions`.

---

## Shipped — 4. State

```csharp
namespace Plugin.Maui.MVVMExpress.State;

public enum ViewModelStatus
{
    Idle, Loading, Refreshing, Saving, Success, Empty, Error, Offline, Unauthorized, Cancelled
}

public sealed class AsyncState<T> : ObservableModel
{
    public ViewModelStatus Status { get; }
    public T? Data { get; }
    public string? Error { get; }
    public Exception? Exception { get; }
    public DateTimeOffset? Timestamp { get; }

    public bool IsLoading { get; }
    public bool IsRefreshing { get; }
    public bool IsEmpty { get; }
    public bool HasError { get; }
    public bool IsSuccess { get; }

    public Task<T> LoadAsync(
        Func<CancellationToken, Task<T>> loader,
        CancellationToken cancellationToken = default);

    public Task<T> RefreshAsync(
        Func<CancellationToken, Task<T>> loader,
        CancellationToken cancellationToken = default);
}
```

`LoadAsync` / `RefreshAsync` return `Task<T>`, not `Outcome<T>`. Failures set `Status` to `Error` or `Cancelled` and **rethrow**. `Error` is a user-facing string, not `ErrorInfo`. There is no `LoadState<T>` or `IStateMachine<T>` type.

---

## Shipped — 5. Outcome

```csharp
namespace Plugin.Maui.MVVMExpress.Outcome;

public readonly struct Outcome
{
    public bool IsSuccess { get; }
    public ErrorInfo? Error { get; }

    public static Outcome Success();
    public static Outcome Failure(ErrorInfo error);
    public static Outcome Failure(string code, string message, Exception? exception = null);
}

public readonly struct Outcome<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public ErrorInfo? Error { get; }

    public static Outcome<T> Success(T value);
    public static Outcome<T> Failure(ErrorInfo error);
    public static Outcome<T> Failure(string code, string message, Exception? exception = null);
}

public sealed class ErrorInfo
{
    public ErrorInfo(string code, string message, Exception? exception = null);

    public string Code { get; }
    public string Message { get; }
    public Exception? Exception { get; }
    public IReadOnlyList<ValidationMessage>? Validation { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

public sealed record ValidationMessage(string PropertyName, string Message);
```

Named `Outcome` so it does not fight FluentResults, LanguageExt, or app-level `Result<T>`.

---

## Shipped — 6. Navigation

```csharp
namespace Plugin.Maui.MVVMExpress.Navigation;

public interface INavigator
{
    Type? Current { get; }
    IReadOnlyList<Type> Stack { get; }
    IReadOnlyList<Type> ModalStack { get; }
    bool CanGoBack { get; }
    IReadOnlyList<NavigationRequest> History { get; }

    Task<Outcome> NavigateToAsync<TViewModel>(
        CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel;

    Task<Outcome> NavigateToAsync<TViewModel, TArgs>(
        TArgs args,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull;

    Task<Outcome> NavigateToAsync(
        string route,
        IReadOnlyDictionary<string, object>? query = null,
        NavOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<Outcome> GoBackAsync(CancellationToken cancellationToken = default);
    Task<Outcome> PopToRootAsync(CancellationToken cancellationToken = default);
    Task<Outcome> ReplaceAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel;
    Task<Outcome> ResetAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel;
}

public interface IPageNavigator : INavigator
{
    IWindowContext Window { get; }
    Task<Outcome> ReplaceRootAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel; // default → ResetAsync
}

public interface IAcceptNavArgs<TArgs> where TArgs : notnull
{
    void Accept(TArgs args);
}

public interface IAcceptNavQuery
{
    void Accept(IReadOnlyDictionary<string, object> query);
}

public sealed class NavOptions
{
    public bool Modal { get; init; }
    public bool Animated { get; init; } = true;
    public bool Replace { get; init; }
}

public sealed record NavigationRequest(
    Type ViewModelType,
    object? Args,
    string? Route = null,
    IReadOnlyDictionary<string, object>? Query = null,
    bool Modal = false);

public interface IWindowContext
{
    string WindowId { get; }
}

public sealed class WindowContext : IWindowContext
{
    public static WindowContext Default { get; }
    public WindowContext(string windowId);
    public string WindowId { get; }
}

public interface IWindowNavigatorRegistry
{
    IWindowContext CurrentWindow { get; set; }
    void Register(IWindowContext window, INavigator navigator);
    INavigator GetNavigator(IWindowContext window);
    INavigator GetCurrent();
    bool TryGetNavigator(IWindowContext window, out INavigator? navigator);
}

public interface IRouteResolver
{
    bool TryResolve(string route, out Type viewModelType);
}

public sealed class GuardedNavigator : IPageNavigator
{
    public GuardedNavigator(INavigator inner, IAuthState auth, params Type[] protectedTypes);
}

public class InMemoryNavigator : IPageNavigator, IRouteResolver
{
    public InMemoryNavigator(Func<Type, bool>? canLeave = null, IWindowContext? window = null);
    public InMemoryNavigator Map<TViewModel>(string route) where TViewModel : class, IViewModel;
}

public sealed class MauiShellNavigator : INavigator, IRouteResolver
{
    public MauiShellNavigator Map<TViewModel>(string route) where TViewModel : class, IViewModel;
}

public sealed class MauiPageNavigator : IPageNavigator, IRouteResolver
{
    public MauiPageNavigator(
        IWindowContext? window = null,
        IServiceProvider? services = null,
        Func<INavigation?>? navigation = null);

    public MauiPageNavigator Map<TViewModel, TPage>(string? route = null)
        where TViewModel : class, IViewModel
        where TPage : Page;
}
```

`Current` / `Stack` / `ModalStack` are ViewModel **types**, not instances. `IAcceptNavArgs<T>` is `void Accept(TArgs)` — not a stored `Args` property. There is no `NavigationContext`, `INavigationHost`, or `NavigateForResultAsync`.

Guards: `CanNavigateAwayAsync` → host navigate → `OnNavigatedFromAsync` / `OnNavigatedToAsync`. `GuardedNavigator` fails as `Outcome` (`E_AUTH`) when a protected type is requested and `IAuthState.IsAuthenticated` is false.

`NavigationRouteTable.FormatQuery` / `ParseQuery` / `Split` / `MergeQuery` exist for URI interop. Host window helpers: `MauiWindowContext.Current` / `For(Window)`, `MauiVisualTree`.

---

## Shipped — 7. Dialogs and notifications

```csharp
namespace Plugin.Maui.MVVMExpress.Dialogs;

public interface IDialogs
{
    Task AlertAsync(string title, string message, string cancel = "OK", CancellationToken cancellationToken = default);
    Task<bool> ConfirmAsync(string title, string message, string accept = "OK", string cancel = "Cancel", CancellationToken cancellationToken = default);
    Task ErrorAsync(ErrorInfo error, CancellationToken cancellationToken = default);
}

public interface INotifier
{
    Task ToastAsync(string message, TimeSpan? duration = null, CancellationToken cancellationToken = default);
}

public interface IToastPresenter
{
    Task ShowAsync(string message, TimeSpan duration, CancellationToken cancellationToken = default);
}

public sealed class NullDialogs : IDialogs, INotifier
{
    public static NullDialogs Instance { get; }
}

public sealed class MauiDialogs : IDialogs
{
    public MauiDialogs(IWindowContext? window = null);
}

public sealed class MauiNotifier : INotifier
{
    public MauiNotifier(IToastPresenter? presenter = null, IWindowContext? window = null);
}

public sealed class MauiToastPresenter : IToastPresenter { }
```

`MauiToastPresenter` draws on `Window.AddOverlay`. It never wraps or replaces `Page.Content`. ViewModels must not call `Page.DisplayAlert`. Prompt, action sheet, loading overlay, snackbar, and banner are **proposed**.

---

## Shipped — 8. Validation

```csharp
namespace Plugin.Maui.MVVMExpress.Validation;

public interface IValidator
{
    Task<ValidationSummary> ValidateAsync(object instance, CancellationToken cancellationToken = default);
    Task<ValidationSummary> ValidatePropertyAsync(object instance, string propertyName, CancellationToken cancellationToken = default);
    ValidationSummary Validate(object instance);
}

public sealed class ValidationSummary
{
    public ValidationSummary(IReadOnlyList<ValidationMessage> messages);
    public bool IsValid { get; }
    public IReadOnlyList<ValidationMessage> Messages { get; }
    public static ValidationSummary Valid { get; }
}

public sealed class DataAnnotationsValidator : IValidator { }
```

FluentValidation: `IValidator` adapter implemented in the app — **not** a PackageReference of `Plugin.Maui.MVVMExpress.Validation`.

XAML `Validation.For` remains [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation). `FormViewModel`, `FormField<T>`, and `IDirtyState` ship in Core (Phase 3).

---

## Shipped — 9. Messaging

```csharp
namespace Plugin.Maui.MVVMExpress.Messaging;

public interface IMessageHub
{
    IDisposable Subscribe<TRecipient, TMessage>(
        TRecipient subscriber,
        Action<TRecipient, TMessage> handler,
        bool weak = true)
        where TRecipient : class;

    void Publish<TMessage>(TMessage message);
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
    void Unsubscribe(object subscriber);
}
```

Default subscribe is weak. The handler must use the recipient argument so the delegate does not capture `this`. `RequestAsync<TRequest, TResponse>` is **proposed**. CommunityToolkit `IMessenger` is not implemented under the same name.

---

## Shipped — 10. Pagination, search, collections

```csharp
namespace Plugin.Maui.MVVMExpress.Pagination;

public abstract class PagedCollection<T> : ObservableModel
{
    protected PagedCollection(int pageSize = 20);

    public int PageSize { get; }
    public AsyncState<IReadOnlyList<T>> State { get; }
    public ObservableRangeCollection<T> Items { get; }
    public bool HasMore { get; }

    public Task LoadMoreAsync(CancellationToken cancellationToken = default);
    public Task RefreshAsync(CancellationToken cancellationToken = default);
    public Task RetryAsync(CancellationToken cancellationToken = default);

    protected abstract Task<IReadOnlyList<T>> FetchAsync(
        int skip, int take, CancellationToken cancellationToken);
}

public sealed class DelegatePagedCollection<T> : PagedCollection<T>
{
    public DelegatePagedCollection(
        Func<int, int, CancellationToken, Task<IReadOnlyList<T>>> fetch,
        int pageSize = 20);
}

public sealed class SearchQuery : ObservableModel
{
    public SearchQuery(TimeSpan? debounce = null, int minimumLength = 0);

    public TimeSpan Debounce { get; }          // default 300 ms
    public int MinimumLength { get; }
    public string Text { get; set; }           // bind Entry, not SearchBar
    public string CommittedText { get; }       // filter after debounce
    public bool IsReady { get; }

    public Task<bool> WhenReadyAsync(CancellationToken cancellationToken = default);
    public void Cancel();
}

public sealed class SnapshotCollection<T> : ObservableModel
{
    public SnapshotCollection(Func<CancellationToken, Task<IReadOnlyList<T>>> fetch);
    public AsyncState<IReadOnlyList<T>> State { get; }
    public ObservableRangeCollection<T> Items { get; }
    public bool IsLoaded { get; }
    public Task LoadAsync(bool force = false, CancellationToken cancellationToken = default);
    public void Replace(IEnumerable<T> items);
    public void AddLocal(T item);
}

namespace Plugin.Maui.MVVMExpress.Collections;

public class ObservableRangeCollection<T> : ObservableCollection<T>
{
    public void AddRange(IEnumerable<T> items);       // one Reset
    public void RemoveRange(IEnumerable<T> items);
    public void ReplaceRange(IEnumerable<T> items);
    public void Reset();
}
```

The page loader is `(skip, take, token)`, not a 1-based page number.

---

## Shipped — 11. Cross-cutting abstractions

```csharp
public interface IBusyGate
{
    bool IsBusy { get; }
    int Depth { get; }
    IDisposable Enter();
}

public interface IErrorSink
{
    Task HandleAsync(ErrorInfo error, CancellationToken cancellationToken = default);
}

public interface IMainThread
{
    bool IsMainThread { get; }
    void BeginInvoke(Action action);
    Task InvokeAsync(Action action, CancellationToken cancellationToken = default);
    Task InvokeAsync(Func<Task> action, CancellationToken cancellationToken = default);
}

public interface IConnectivityProbe
{
    bool IsOnline { get; }
}

public interface ICache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

public interface IAuthState
{
    bool IsAuthenticated { get; }
    string? UserName { get; }
    Task<Outcome> SignInAsync(string userName, string password, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
}
```

`IConnectivityProbe` has no `ConnectionType` / `ConnectionChanged`. `ICache.SetAsync` has no entry-options overload. `IRetryPolicy` and `IMvvmExpressTelemetry` remain **proposed**.

Default Host implementations may wrap in-memory stand-ins. Production apps should swap:

| Abstraction | Suggested adapter |
| --- | --- |
| `IConnectivityProbe` | Plugin.Maui.NetworkMonitor |
| `ICache` | Plugin.Maui.ApiCache |
| `IAuthState` | Plugin.Maui.SecureSession |
| Feature flags (`IFeatureSwitch`) | Plugin.Maui.FeatureFlags |
| Permissions (`IPermissionGate`) | Plugin.Maui.PermissionFlow |

---

## Shipped — 12. Testing

```csharp
namespace Plugin.Maui.MVVMExpress.Testing;

public sealed class FakeNavigator : InMemoryNavigator
{
    public FakeNavigator(Func<Type, bool>? canLeave = null, IWindowContext? window = null);
}

public sealed class FakeDialogs : IDialogs, INotifier
{
    public List<string> Alerts { get; }
    public bool ConfirmResult { get; set; }
}

public sealed class FakeMainThread : IMainThread { public int InvokeCount { get; } }
public sealed class FakeConnectivity : IConnectivityProbe { public bool IsOnline { get; set; } }
public sealed class FakeMessageHub : IMessageHub { public List<object?> Published { get; } }

public static class ViewModelLifecycle
{
    public static Task AppearAsync(this IViewModel viewModel, CancellationToken cancellationToken = default);
    public static Task DisappearAsync(this IViewModel viewModel, CancellationToken cancellationToken = default);
}

public sealed class ScopedNavigator : IDisposable
{
    public ScopedNavigator(IViewModelScopeFactory factory);
    public IViewModel? Current { get; }
    public int Count { get; }
    public bool CanGoBack { get; }
    public TViewModel Push<TViewModel>(Action<TViewModel>? configure = null) where TViewModel : class, IViewModel;
    public void Pop();
}

public static class LeakProbe
{
    public static bool IsCollected(WeakReference reference, int rounds = 3);
    public static WeakReference Track(object target);
}

public enum ApplicationScale { Small, Mid, Large }

public static class ScaleProfile
{
    public static int ListSize(ApplicationScale scale);
    public static int ViewModelBatch(ApplicationScale scale);
}
```

There is no `FakeNotifier` type — `FakeDialogs` is `INotifier`. Inject `IToastPresenter` to record toasts without a window. There is no `ViewModelTest` helper class. First `AppearAsync` calls `InitializeAsync` once (same order as `ViewModelLifecycleBehavior`).

---

## Shipped — 13. Forms, cache policies, pipeline, scopes (0.4.0)

```csharp
namespace Plugin.Maui.MVVMExpress.Forms;

public interface IDirtyState { bool IsDirty { get; } void MarkClean(); void Reset(); }
public sealed class FormField<T> : ObservableModel, IFormField { }
public abstract class FormViewModel : PageViewModel, IDirtyState
{
    protected void Bind<T>(FormField<T> field, string propertyName, Action? notifyCanExecute = null);
}
public sealed class UndoStack : ObservableModel { }

namespace Plugin.Maui.MVVMExpress.Caching;

public enum FetchPolicy { CacheFirst, NetworkFirst, StaleWhileRevalidate, NetworkOnly, CacheOnly }
public interface ICachedFetcher { Task<CachedFetchResult<T>> FetchAsync<T>(...); }
public sealed class CachedFetcher : ICachedFetcher { }

namespace Plugin.Maui.MVVMExpress.Operations;

public interface IOperationExecutor { Task<Outcome<T>> RunAsync<T>(...); }
public sealed class OperationOptions { }

namespace Plugin.Maui.MVVMExpress.Composition;

public interface IViewModelComposer { TChild Attach<TChild>(TChild child); }
public interface IViewModelScopeFactory { IViewModelScope CreatePageScope(); }
public interface ISectionHost { string CurrentKey { get; } IViewModel? Current { get; } Task SelectAsync(string key, CancellationToken cancellationToken = default); }
public class SectionHostViewModel : PageViewModel, ISectionHost { }

namespace Plugin.Maui.MVVMExpress.Reactive;

public interface IPropertyObservable<out T> : IDisposable { }
public static class PropertyObservable
{
    public static IPropertyObservable<T> Observe<T>(...);
    public static IPropertyObservable<TResult> CombineLatest<T1, T2, TResult>(...);
}
```

`FormViewModel.CanNavigateAwayAsync` returns `false` while `IsDirty`. `IFeatureSwitch`, `IPermissionGate`, `IFileStore`, and `IMediaPicker` ship as in-memory / no-op defaults.

---

## Proposed — later phases

Do not copy these into app code. They are not packed.

### Hosting (later)

```csharp
// Not shipped
services.AddViewModel<TViewModel>(ServiceLifetime lifetime = Transient);
services.AddView<TView, TViewModel>(...);

public sealed class MvvmExpressOptions
{
    // Only CancelOperationsOnDisappear exists today.
    public bool EnableNavigation { get; set; } = true;
    public bool EnableLifecycle { get; set; } = true;
    public bool EnableAutoRegistration { get; set; } = false;
    // EnableDiagnostics exists on the shipped options (Debug-only wiring).
    public bool EnableReactive { get; set; } = false;
}
```

### Commands (later)

```csharp
public sealed class AsyncCommandOptions
{
    public ErrorHandling ErrorHandling { get; init; }
}

public class CompositeModelCommand : ICommand { }
```

### Navigation (later)

```csharp
public sealed class NavigationContext { /* Source, Target, Args, Query, Cancel */ }

Task<Outcome<TResult>> NavigateForResultAsync<TViewModel, TResult>(...);
```

### Dialogs (later)

```csharp
Task<string?> PromptAsync(...);
Task<string?> ActionSheetAsync(...);
Task<IDisposable> ShowLoadingAsync(...);
Task SnackbarAsync(...);
Task BannerAsync(...);
```

### State machine (later)

```csharp
public interface IStateMachine<TState> where TState : struct, Enum { }
```

### Messaging (later)

```csharp
Task<TResponse> RequestAsync<TRequest, TResponse>(...);
```

### Later (not this release)

```csharp
services.AddViewModel<TViewModel>(...);
public class CompositeModelCommand : ICommand { }
Task<Outcome<TResult>> NavigateForResultAsync<...>(...);
```

---

## Shipped — 14. Generators, persist, auth policy (0.5.0)

```csharp
[Notify] [NotifyAlso("Label")]
[ModelCommand] / [AsyncModelCommand]
[RegisterViewModel] / [RegisterView]
[Route("details")]
[PersistState]
[RequiresAuth] / [RequiresRole("admin")]
```

Generated `Plugin.Maui.MVVMExpress.Generated.MvvmExpressGeneratedRegistrations` adds ViewModels and routes without a reflection scan. Handwritten `SetProperty` and `Map<TViewModel>` remain valid. `CommunityToolkitMessageHub` lives in the Compatibility package (does not type-forward `IMessenger`).

---

## Explicitly not public

- Prism `INavigationService`, `IDialogService`, `INavigationParameters`
- CommunityToolkit `ObservableObject`, `RelayCommand`, `[ObservableProperty]`, `IMessenger` (use Compatibility `CommunityToolkitMessageHub` to adapt)
- ReactiveUI `ReactiveObject`, `ReactiveCommand`, `WhenAnyValue`
- Static `MVVMExpress.Current` in Core
- Types that reference `Page` or `Shell` inside Core
- Prism-style regions (deferred past 1.0)
