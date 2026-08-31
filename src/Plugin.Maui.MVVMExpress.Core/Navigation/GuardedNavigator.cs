using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>Blocks navigation to selected ViewModel types until <see cref="IAuthState.IsAuthenticated"/>.</summary>
public sealed class GuardedNavigator : INavigator
{
    private readonly INavigator _inner;
    private readonly IAuthState _auth;
    private readonly HashSet<Type> _protectedTypes;

    /// <summary>Creates a guard around <paramref name="inner"/>.</summary>
    public GuardedNavigator(INavigator inner, IAuthState auth, params Type[] protectedTypes)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(auth);
        _inner = inner;
        _auth = auth;
        _protectedTypes = [.. protectedTypes];
    }

    /// <inheritdoc />
    public Type? Current => _inner.Current;

    /// <inheritdoc />
    public IReadOnlyList<NavigationRequest> History => _inner.History;

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => Gate<TViewModel>(() => _inner.NavigateToAsync<TViewModel>(cancellationToken));

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull
        => Gate<TViewModel>(() => _inner.NavigateToAsync<TViewModel, TArgs>(args, cancellationToken));

    /// <inheritdoc />
    public Task<Result> GoBackAsync(CancellationToken cancellationToken = default)
        => _inner.GoBackAsync(cancellationToken);

    private Task<Result> Gate<TViewModel>(Func<Task<Result>> next)
    {
        if (_protectedTypes.Contains(typeof(TViewModel)) && !_auth.IsAuthenticated)
        {
            return Task.FromResult(Result.Failure("E_AUTH", "Sign in required"));
        }

        return next();
    }
}
