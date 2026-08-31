# MVVMExpress Public API Design

Proposed public surface for **Plugin.Maui.MVVMExpress**. **Shipped in Core (with tests):** `ObservableModel`, `IViewModel`, `ViewModel`, commands (sync/async, generic), `AsyncState<T>`, `Outcome` / `ErrorInfo`, `BusyGate`, `IMessageHub` / `MessageHub`, `ObservableRangeCollection<T>`, `IMainThread` / `ImmediateMainThread`. Hosting, navigation, dialogs, and generators remain design-only. Breaking changes after 1.0 follow SemVer.

Default namespace root: `Plugin.Maui.MVVMExpress`.

## 1. Hosting

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
    public static IServiceCollection AddMvvmExpress(
        this IServiceCollection services,
        Action<MvvmExpressOptions>? configure = null);

    public static IServiceCollection AddViewModel<TViewModel>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TViewModel : class, IViewModel;

    public static IServiceCollection AddView<TView, TViewModel>(
        this IServiceCollection services,
        ServiceLifetime viewLifetime = ServiceLifetime.Transient,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient)
        where TView : class
        where TViewModel : class, IViewModel;
}

public sealed class MvvmExpressOptions
{
    public bool EnableNavigation { get; set; } = true;
    public bool EnableLifecycle { get; set; } = true;
    public bool EnableAutoRegistration { get; set; } = false;
    public bool EnableDiagnostics { get; set; } = false;
    public bool EnableReactive { get; set; } = false;
    public bool CancelOperationsOnDisappear { get; set; } = true;
}
```

`AddMauiMvvm` is **not** used (avoids implying ownership of MAUI’s MVVM). Alias may be added later if review wants it.

## 2. Observable model and ViewModels

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

    protected void Notify(string propertyName);
    protected void NotifyChanging(string propertyName);
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
    Task OnNavigatedToAsync(NavigationContext context, CancellationToken cancellationToken = default);
    Task OnNavigatedFromAsync(NavigationContext context, CancellationToken cancellationToken = default);
    Task<bool> CanNavigateAwayAsync(NavigationContext context, CancellationToken cancellationToken = default);
}

public abstract class ViewModel : ObservableModel, IViewModel
{
    protected ViewModel();
    protected ViewModel(IErrorSink? errors, IBusyGate? busy, ILogger? logger);

    public CancellationToken ViewModelCancellationToken { get; }
    public IReadOnlyCollection<ITrackedOperation> ActiveOperations { get; }

    protected Task<Outcome> ExecuteAsync(
        Func<CancellationToken, Task> operation,
        OperationOptions? options = null,
        CancellationToken cancellationToken = default);

    protected Task<Outcome<T>> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        OperationOptions? options = null,
        CancellationToken cancellationToken = default);
}

public abstract class PageViewModel : ViewModel, INavigable
{
    protected INavigator? Navigator { get; }
    protected IDialogs? Dialogs { get; }
}
```

Manual INPC is always valid. Generators emit partials on types that already inherit `ObservableModel`.

```csharp
// Phase 4 — generated
[Notify]
private string? name;

// emits public string? Name { get; set; }
// partial void OnNameChanging(string? value);
// partial void OnNameChanged(string? value);
```

## 3. Commands

```csharp
namespace Plugin.Maui.MVVMExpress.Input;

public interface IModelCommand : ICommand
{
    bool IsRunning { get; }
    bool IsCancellationRequested { get; }
    CommandExecutionState State { get; } // Idle, Running, Completed, Failed, Cancelled
    void NotifyCanExecuteChanged();
}

public interface IAsyncModelCommand : IModelCommand
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
    void Cancel();
}

public class ModelCommand : IModelCommand { /* Action / Action<T>, CanExecute */ }
public class ModelCommand<T> : IModelCommand { }
public class AsyncModelCommand : IAsyncModelCommand { }
public class AsyncModelCommand<T> : IAsyncModelCommand { }

public sealed class AsyncCommandOptions
{
    public bool AllowConcurrentExecution { get; init; }
    public bool CancelPreviousExecution { get; init; }
    public TimeSpan? Timeout { get; init; }
    public int RetryCount { get; init; }
    public TimeSpan RetryDelay { get; init; }
    public TimeSpan? Debounce { get; init; }
    public TimeSpan? Throttle { get; init; }
    public ConcurrencyMode Concurrency { get; init; } = ConcurrencyMode.Prevent;
    public ErrorHandling ErrorHandling { get; init; } = ErrorHandling.Sink;
}

public enum ConcurrencyMode { Allow, Prevent, Queue, CancelPrevious, Replace }
public enum ErrorHandling { Sink, Throw, Outcome }

public class CompositeModelCommand : IModelCommand { }
```

`ExecuteAsync`, `Cancel`, `CanExecute`, `IsRunning`, and `IsCancellationRequested` are required on async commands.

## 4. State

```csharp
namespace Plugin.Maui.MVVMExpress.State;

public enum ViewModelStatus
{
    Idle,
    Loading,
    Refreshing,
    Saving,
    Success,
    Empty,
    Error,
    Offline,
    Unauthorized,
    Cancelled
}

public class AsyncState<T> : ObservableModel
{
    public ViewModelStatus Status { get; }
    public T? Data { get; }
    public ErrorInfo? Error { get; }
    public Exception? Exception { get; }
    public DateTimeOffset? Timestamp { get; }

    public bool IsLoading { get; }
    public bool IsRefreshing { get; }
    public bool IsEmpty { get; }
    public bool HasError { get; }
    public bool IsSuccess { get; }

    public Task<Outcome<T>> LoadAsync(
        Func<CancellationToken, Task<T>> loader,
        CancellationToken cancellationToken = default);

    public Task<Outcome<T>> RefreshAsync(
        Func<CancellationToken, Task<T>> loader,
        CancellationToken cancellationToken = default);
}

public sealed class LoadState<T> : AsyncState<T> { }

public interface IStateMachine<TState> where TState : struct, Enum
{
    TState Current { get; }
    bool CanTransition(TState next);
    Outcome Transition(TState next);
    void Allow(TState from, TState to);
}
```

Default machine (when enabled):

```
Idle → Loading → Success | Empty | Error | Cancelled | Offline | Unauthorized
Success → Refreshing → Success | Empty | Error | Cancelled
Error → Loading | Refreshing
```

Invalid transitions return `Outcome.Failure` and do not throw unless `ThrowOnInvalidTransition` is set.

## 5. Outcome

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
}

public sealed class ErrorInfo
{
    public string Code { get; init; }
    public string Message { get; init; }
    public Exception? Exception { get; init; }
    public IReadOnlyList<ValidationMessage>? Validation { get; init; }
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}
```

Named `Outcome` so it does not fight FluentResults, LanguageExt, or app-level `Result<T>`.

## 6. Navigation

```csharp
namespace Plugin.Maui.MVVMExpress.Navigation;

public interface INavigator
{
    object? CurrentViewModel { get; }
    IReadOnlyList<object> Stack { get; }
    IReadOnlyList<object> ModalStack { get; }
    bool CanGoBack { get; }

    Task<Outcome> NavigateToAsync<TViewModel>(
        CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel;

    Task<Outcome> NavigateToAsync<TViewModel, TArgs>(
        TArgs args,
        NavOptions? options = null,
        CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull;

    Task<Outcome> NavigateToAsync(
        string route,
        IReadOnlyDictionary<string, object>? query = null,
        NavOptions? options = null,
        CancellationToken cancellationToken = default);

    Task<Outcome<TResult>> NavigateForResultAsync<TViewModel, TResult>(
        CancellationToken cancellationToken = default);

    Task<Outcome> GoBackAsync(NavOptions? options = null, CancellationToken cancellationToken = default);
    Task<Outcome> PopToRootAsync(CancellationToken cancellationToken = default);
    Task<Outcome> ReplaceAsync<TViewModel>(CancellationToken cancellationToken = default);
    Task<Outcome> ResetAsync<TViewModel>(CancellationToken cancellationToken = default);
}

public sealed class NavOptions
{
    public bool Modal { get; init; }
    public bool Animated { get; init; } = true;
    public bool Replace { get; init; }
}

public sealed class NavigationContext
{
    public object? Source { get; }
    public object? Target { get; }
    public object? Args { get; }
    public IReadOnlyDictionary<string, object> Query { get; }
    public bool IsCancelled { get; }
    public void Cancel();
}

public interface IAcceptNavArgs<TArgs> where TArgs : notnull
{
    TArgs Args { get; }
}

public interface INavigationHost
{
    // Implemented by ShellNavigationHost and PageNavigationHost
}
```

Guards run in this order: `OnNavigating` → `CanNavigateAwayAsync` → host navigate → `OnNavigatedFrom` / `OnNavigatedTo`.

## 7. Dialogs and notifications

```csharp
namespace Plugin.Maui.MVVMExpress.Dialogs;

public interface IDialogs
{
    Task AlertAsync(string title, string message, string cancel = "OK", CancellationToken cancellationToken = default);
    Task<bool> ConfirmAsync(string title, string message, string accept = "OK", string cancel = "Cancel", CancellationToken cancellationToken = default);
    Task<string?> PromptAsync(string title, string message, string accept = "OK", string cancel = "Cancel", string? placeholder = null, CancellationToken cancellationToken = default);
    Task<string?> ActionSheetAsync(string title, string cancel, string? destruction, IEnumerable<string> buttons, CancellationToken cancellationToken = default);
    Task<IDisposable> ShowLoadingAsync(string? message = null, CancellationToken cancellationToken = default);
    Task ErrorAsync(ErrorInfo error, CancellationToken cancellationToken = default);
}

public interface INotifier
{
    Task ToastAsync(string message, TimeSpan? duration = null, CancellationToken cancellationToken = default);
    Task SnackbarAsync(string message, string? action = null, Func<CancellationToken, Task>? onAction = null, CancellationToken cancellationToken = default);
    Task BannerAsync(string message, CancellationToken cancellationToken = default);
}
```

ViewModels must not call `Page.DisplayAlert`.

## 8. Validation and forms

```csharp
namespace Plugin.Maui.MVVMExpress.Validation;

public interface IValidator
{
    Task<ValidationSummary> ValidateAsync(object instance, CancellationToken cancellationToken = default);
    Task<ValidationSummary> ValidatePropertyAsync(object instance, string propertyName, CancellationToken cancellationToken = default);
}

public sealed class ValidationSummary
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationMessage> Messages { get; }
}

public abstract class FormViewModel : PageViewModel
{
    public IDirtyState Dirty { get; }
    public ValidationSummary Validation { get; }
    public Task<Outcome> SubmitAsync(CancellationToken cancellationToken = default);
    public void Reset();
}

public sealed class FormField<T> : ObservableModel
{
    public T? Value { get; set; }
    public bool IsTouched { get; }
    public bool IsDirty { get; }
    public bool HasError { get; }
    public string? Error { get; }
}

public interface IDirtyState
{
    DirtyStatus Status { get; } // Clean, Dirty, Saving, Saved
    bool HasUnsavedChanges { get; }
}
```

FluentValidation: `IValidator` adapter implemented in the app or an optional extra; **not** a PackageReference of `Plugin.Maui.MVVMExpress.Validation`.

XAML `Validation.For` remains [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation).

## 9. Messaging

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

// Designed, not shipped: RequestAsync<TRequest, TResponse>

```

CommunityToolkit `IMessenger` is adapted in a later compatibility package, not implemented under the same name.

## 10. Pagination, refresh, search

```csharp
namespace Plugin.Maui.MVVMExpress.Pagination;

public abstract class PagedCollection<T> : ObservableModel
{
    public AsyncState<IReadOnlyList<T>> State { get; }
    public bool HasMore { get; }
    public Task LoadMoreAsync(CancellationToken cancellationToken = default);
    public Task RefreshAsync(CancellationToken cancellationToken = default);
    public Task RetryAsync(CancellationToken cancellationToken = default);
}

public sealed class SearchQuery : ObservableModel
{
    public string Text { get; set; }
    public TimeSpan Debounce { get; init; } = TimeSpan.FromMilliseconds(300);
    public int MinimumLength { get; init; } = 2;
}
```

## 11. Cross-cutting abstractions

```csharp
public interface IBusyGate
{
    IDisposable Enter();
    bool IsBusy { get; }
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
    string? ConnectionType { get; }
    event EventHandler? ConnectionChanged;
}

public interface ICache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}

public interface IRetryPolicy
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default);
}

public interface IAuthState
{
    bool IsAuthenticated { get; }
    bool IsExpired { get; }
}

public interface IFeatureSwitch
{
    bool IsEnabled(string name);
}

public interface IPermissionGate
{
    Task<PermissionState> CheckAsync(string permission, CancellationToken cancellationToken = default);
    Task<PermissionState> RequestAsync(string permission, CancellationToken cancellationToken = default);
}

public interface IMvvmExpressTelemetry
{
    void Track(string name, IReadOnlyDictionary<string, object?>? tags = null, TimeSpan? duration = null);
}
```

Default Host implementations may wrap MAUI Essentials. Production apps should swap:

| Abstraction | Suggested adapter |
| --- | --- |
| `IConnectivityProbe` | Plugin.Maui.NetworkMonitor |
| `ICache` | Plugin.Maui.ApiCache |
| `IAuthState` | Plugin.Maui.SecureSession |
| `IFeatureSwitch` | Plugin.Maui.FeatureFlags |
| `IPermissionGate` | Plugin.Maui.PermissionFlow |

## 12. Source generator attributes (Phase 4)

```csharp
namespace Plugin.Maui.MVVMExpress.ComponentModel;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class NotifyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class NotifyAlsoAttribute : Attribute
{
    public NotifyAlsoAttribute(params string[] propertyNames) { }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ModelCommandAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class AsyncModelCommandAttribute : Attribute
{
    public ConcurrencyMode Concurrency { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisterViewModelAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisterViewAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class RouteAttribute : Attribute
{
    public RouteAttribute(string path) { }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PersistStateAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class RequiresAuthAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class RequiresRoleAttribute : Attribute
{
    public RequiresRoleAttribute(string role) { }
}
```

## 13. Testing

```csharp
namespace Plugin.Maui.MVVMExpress.Testing;

public class FakeNavigator : INavigator { }
public class FakeDialogs : IDialogs { }
public class FakeNotifier : INotifier { }
public class FakeMainThread : IMainThread { } // runs inline
public class FakeConnectivity : IConnectivityProbe { }
public class FakeMessageHub : IMessageHub { }

public static class ViewModelTest
{
    public static Task AppearAsync(IViewModel vm, CancellationToken cancellationToken = default);
    public static Task DisappearAsync(IViewModel vm, CancellationToken cancellationToken = default);
}
```

## 14. Explicitly not public

- Prism `INavigationService`, `IDialogService`, `INavigationParameters`
- CommunityToolkit `ObservableObject`, `RelayCommand`, `[ObservableProperty]`, `IMessenger` (unless Compatibility package)
- ReactiveUI `ReactiveObject`, `ReactiveCommand`, `WhenAnyValue`
- Static `MVVMExpress.Current` in Core
- Types that reference `Page` or `Shell` inside Core
