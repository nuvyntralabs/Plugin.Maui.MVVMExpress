using Plugin.Maui.MVVMExpress.ComponentModel;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>
/// Records navigation for tests and samples that do not yet attach a MAUI Shell host.
/// </summary>
public class InMemoryNavigator : INavigator
{
    private readonly List<NavigationRequest> _history = [];
    private readonly Func<Type, bool>? _canLeave;

    /// <summary>Creates a recorder. Optional <paramref name="canLeave"/> is a dirty-page guard.</summary>
    /// <param name="canLeave">When the current type is set, return false to block.</param>
    public InMemoryNavigator(Func<Type, bool>? canLeave = null) => _canLeave = canLeave;

    /// <inheritdoc />
    public Type? Current { get; set; }

    /// <inheritdoc />
    public IReadOnlyList<NavigationRequest> History => _history;

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => NavigateCore(typeof(TViewModel), null, cancellationToken);

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull
        => NavigateCore(typeof(TViewModel), args, cancellationToken);

    /// <inheritdoc />
    public Task<Result> GoBackAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _history.Add(new NavigationRequest(typeof(object), "back"));
        return Task.FromResult(Result.Success());
    }

    private Task<Result> NavigateCore(Type viewModelType, object? args, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (Current is not null && _canLeave?.Invoke(Current) == false)
        {
            return Task.FromResult(Result.Failure("E_GUARD", "Navigation blocked"));
        }

        _history.Add(new NavigationRequest(viewModelType, args));
        Current = viewModelType;
        return Task.FromResult(Result.Success());
    }
}
