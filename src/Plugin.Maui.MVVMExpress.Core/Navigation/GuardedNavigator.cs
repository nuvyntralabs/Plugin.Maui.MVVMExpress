using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>Blocks navigation to selected ViewModel types until <see cref="IAuthState.IsAuthenticated"/>.</summary>
public sealed class GuardedNavigator : IPageNavigator
{
    private readonly INavigator _inner;
    private readonly IAuthState _auth;
    private readonly HashSet<Type> _protectedTypes;
    private readonly INavigationAuthPolicy? _policy;

    /// <summary>Creates a guard around <paramref name="inner"/>.</summary>
    public GuardedNavigator(INavigator inner, IAuthState auth, params Type[] protectedTypes)
        : this(inner, auth, policy: null, protectedTypes)
    {
    }

    /// <summary>Creates a guard that also applies <paramref name="policy"/> (generated <see cref="RequiresAuthAttribute"/> maps).</summary>
    public GuardedNavigator(INavigator inner, IAuthState auth, INavigationAuthPolicy? policy, params Type[] protectedTypes)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(auth);
        _inner = inner;
        _auth = auth;
        _policy = policy;
        _protectedTypes = [.. protectedTypes];
    }

    /// <inheritdoc />
    public IWindowContext Window => _inner is IPageNavigator page ? page.Window : WindowContext.Default;

    /// <inheritdoc />
    public Type? Current => _inner.Current;

    /// <inheritdoc />
    public IReadOnlyList<Type> Stack => _inner.Stack;

    /// <inheritdoc />
    public IReadOnlyList<Type> ModalStack => _inner.ModalStack;

    /// <inheritdoc />
    public bool CanGoBack => _inner.CanGoBack;

    /// <inheritdoc />
    public IReadOnlyList<NavigationRequest> History => _inner.History;

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => Gate(typeof(TViewModel), () => _inner.NavigateToAsync<TViewModel>(cancellationToken));

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull
        => Gate(typeof(TViewModel), () => _inner.NavigateToAsync<TViewModel, TArgs>(args, cancellationToken));

    /// <inheritdoc />
    public Task<Result> NavigateToAsync(
        string route,
        IReadOnlyDictionary<string, object>? query = null,
        NavOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (_inner is IRouteResolver resolver && resolver.TryResolve(route, out var type))
        {
            return Gate(type, () => _inner.NavigateToAsync(route, query, options, cancellationToken));
        }

        return _inner.NavigateToAsync(route, query, options, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result> GoBackAsync(CancellationToken cancellationToken = default)
        => _inner.GoBackAsync(cancellationToken);

    /// <inheritdoc />
    public Task<Result> PopToRootAsync(CancellationToken cancellationToken = default)
        => _inner.PopToRootAsync(cancellationToken);

    /// <inheritdoc />
    public Task<Result> ReplaceAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => Gate(typeof(TViewModel), () => _inner.ReplaceAsync<TViewModel>(cancellationToken));

    /// <inheritdoc />
    public Task<Result> ResetAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => Gate(typeof(TViewModel), () => _inner.ResetAsync<TViewModel>(cancellationToken));

    private Task<Result> Gate(Type viewModelType, Func<Task<Result>> next)
    {
        var requiresAuth = _protectedTypes.Contains(viewModelType)
            || _policy?.RequiresAuthentication(viewModelType) == true;
        if (requiresAuth && !_auth.IsAuthenticated)
        {
            return Task.FromResult(Result.Failure("E_AUTH", "Sign in required"));
        }

        if (_policy?.RequiresRole(viewModelType, out var role) == true
            && !string.IsNullOrEmpty(role)
            && (_auth is not IRoleState roles || !roles.HasRole(role)))
        {
            return Task.FromResult(Result.Failure("E_ROLE", $"Role '{role}' required"));
        }

        return next();
    }
}
