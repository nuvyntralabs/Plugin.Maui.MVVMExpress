using System.Reflection;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>
/// <see cref="INavigator"/> that calls <c>Shell.GoToAsync</c>. Map each ViewModel type to a Shell route.
/// </summary>
public sealed class MauiShellNavigator : INavigator
{
    private readonly Dictionary<Type, string> _routes = [];
    private readonly List<NavigationRequest> _history = [];

    /// <inheritdoc />
    public Type? Current { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<NavigationRequest> History => _history;

    /// <summary>Maps <typeparamref name="TViewModel"/> to a Shell route (<c>details</c> or <c>//products</c>).</summary>
    /// <typeparam name="TViewModel">Destination ViewModel.</typeparam>
    /// <param name="route">Absolute (<c>//name</c>) or relative route.</param>
    public MauiShellNavigator Map<TViewModel>(string route)
        where TViewModel : class, IViewModel
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        _routes[typeof(TViewModel)] = route;
        return this;
    }

    /// <summary>Builds a query string from public instance properties on <paramref name="args"/>.</summary>
    /// <param name="args">Typed navigation arguments.</param>
    public static string FormatQuery(object args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var pairs = args.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Select(property =>
                $"{Uri.EscapeDataString(property.Name)}={Uri.EscapeDataString(property.GetValue(args)?.ToString() ?? "")}");
        return string.Join("&", pairs);
    }

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => GoAsync(typeof(TViewModel), null, cancellationToken);

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull
        => GoAsync(typeof(TViewModel), args, cancellationToken);

    /// <inheritdoc />
    public Task<Result> GoBackAsync(CancellationToken cancellationToken = default)
        => GoUriAsync("..", typeof(object), "back", cancellationToken);

    private Task<Result> GoAsync(Type viewModelType, object? args, CancellationToken cancellationToken)
    {
        if (!_routes.TryGetValue(viewModelType, out var route))
        {
            return Task.FromResult(Result.Failure("E_ROUTE", $"No Shell route mapped for {viewModelType.Name}."));
        }

        var uri = args is null ? route : $"{route}?{FormatQuery(args)}";
        return GoUriAsync(uri, viewModelType, args, cancellationToken);
    }

    private async Task<Result> GoUriAsync(string uri, Type viewModelType, object? args, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var shell = Shell.Current;
        if (shell is null)
        {
            return Result.Failure("E_SHELL", "Shell.Current is not available.");
        }

        if (shell.CurrentPage?.BindingContext is INavigable navigable
            && !await navigable.CanNavigateAwayAsync(cancellationToken).ConfigureAwait(true))
        {
            return Result.Failure("E_GUARD", "Navigation blocked");
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(() => shell.GoToAsync(uri)).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            return Result.Failure("E_NAV", ex.Message, ex);
        }

        _history.Add(new NavigationRequest(viewModelType, args));
        Current = viewModelType;
        return Result.Success();
    }
}
