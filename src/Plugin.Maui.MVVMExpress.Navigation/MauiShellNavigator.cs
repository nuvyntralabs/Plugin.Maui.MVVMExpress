using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>
/// <see cref="INavigator"/> that calls <c>Shell.GoToAsync</c>. Map each ViewModel type to a Shell route.
/// </summary>
public sealed class MauiShellNavigator : INavigator, IRouteResolver
{
    private readonly NavigationRouteTable _routes = new();
    private readonly NavigationStack _stack = new();
    private readonly IServiceCollection? _services;

    /// <inheritdoc />
    public Type? Current => _stack.Current;

    /// <inheritdoc />
    public IReadOnlyList<Type> Stack => _stack.Stack;

    /// <inheritdoc />
    public IReadOnlyList<Type> ModalStack => _stack.ModalStack;

    /// <inheritdoc />
    public bool CanGoBack => _stack.CanGoBack;

    /// <inheritdoc />
    public IReadOnlyList<NavigationRequest> History => _stack.History;

    /// <summary>Creates a Shell navigator. Pass <paramref name="services"/> so <see cref="Map{TViewModel,TPage}"/> can register DI.</summary>
    public MauiShellNavigator(IServiceCollection? services = null)
        => _services = services;

    /// <summary>Maps <typeparamref name="TViewModel"/> to a Shell route (<c>details</c> or <c>//products</c>).</summary>
    public MauiShellNavigator Map<TViewModel>(string route)
        where TViewModel : class, IViewModel
    {
        _routes.Map<TViewModel>(route);
        return this;
    }

    /// <summary>
    /// Maps a ViewModel to a page type: Shell route, <c>Routing.RegisterRoute</c>, and transient DI.
    /// Prefer <see cref="CreateContent{TPage}"/> so Shell does not capture a singleton page instance.
    /// </summary>
    public MauiShellNavigator Map<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TPage>(string route)
        where TViewModel : class, IViewModel
        where TPage : Page
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        _routes.Map<TViewModel>(route);
        var path = route.TrimStart('/');
        if (path.StartsWith("//", StringComparison.Ordinal))
        {
            path = path[2..];
        }

        Routing.RegisterRoute(path, typeof(TPage));
        _services?.TryAddTransient<TViewModel>();
        _services?.TryAddTransient<TPage>();
        return this;
    }

    /// <summary>Creates a <see cref="ShellContent"/> that resolves <typeparamref name="TPage"/> per navigation.</summary>
    public static ShellContent CreateContent<TPage>(string route, IServiceProvider services)
        where TPage : Page
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentNullException.ThrowIfNull(services);
        var path = route.TrimStart('/');
        if (path.StartsWith("//", StringComparison.Ordinal))
        {
            path = path[2..];
        }

        return new ShellContent
        {
            Route = path,
            ContentTemplate = new DataTemplate(() => services.GetRequiredService<TPage>())
        };
    }

    /// <inheritdoc />
    public bool TryResolve(string route, out Type viewModelType) => _routes.TryResolve(route, out viewModelType);

    /// <summary>Builds a query string from public instance properties or a dictionary.</summary>
    public static string FormatQuery(object args) => NavigationRouteTable.FormatQuery(args);

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => GoAsync(typeof(TViewModel), null, null, null, cancellationToken);

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull
        => GoAsync(typeof(TViewModel), args, null, args as IReadOnlyDictionary<string, object>, cancellationToken);

    /// <inheritdoc />
    public Task<Result> NavigateToAsync(
        string route,
        IReadOnlyDictionary<string, object>? query = null,
        NavOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        var split = NavigationRouteTable.Split(route);
        var merged = NavigationRouteTable.MergeQuery(split.Query, query);
        _routes.TryResolve(split.Path, out var viewModelType);
        if (!_routes.TryGetRoute(viewModelType ?? typeof(object), out var mapped))
        {
            mapped = split.Path;
        }

        var targetType = viewModelType ?? typeof(object);
        return GoUriAsync(BuildUri(mapped, merged), targetType, merged, mapped, merged, options, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result> NavigateToAsync(Type viewModelType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModelType);
        return GoAsync(viewModelType, null, null, null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result> GoBackAsync(CancellationToken cancellationToken = default)
        => GoUriAsync("..", typeof(object), "back", "..", null, null, cancellationToken, pop: true);

    /// <inheritdoc />
    public async Task<Result> PopToRootAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var shell = Shell.Current;
        if (shell is null)
        {
            return Result.Failure("E_SHELL", "Shell.Current is not available.");
        }

        if (await IsBlockedAsync(cancellationToken).ConfigureAwait(true))
        {
            return Result.Failure("E_GUARD", "Navigation blocked");
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(() => shell.Navigation.PopToRootAsync()).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            return Result.Failure("E_NAV", ex.Message, ex);
        }

        _stack.PopToRoot();
        return Result.Success();
    }

    /// <inheritdoc />
    public Task<Result> ReplaceAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => GoAsync(typeof(TViewModel), null, null, null, cancellationToken, new NavOptions { Replace = true });

    /// <inheritdoc />
    public Task<Result> ResetAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
    {
        if (!_routes.TryGetRoute(typeof(TViewModel), out var route))
        {
            return Task.FromResult(Result.Failure("E_ROUTE", $"No Shell route mapped for {typeof(TViewModel).Name}."));
        }

        var absolute = route.StartsWith("//", StringComparison.Ordinal) ? route : $"//{route.TrimStart('/')}";
        return GoUriAsync(absolute, typeof(TViewModel), null, route, null, null, cancellationToken, reset: true);
    }

    private Task<Result> GoAsync(
        Type viewModelType,
        object? args,
        string? route,
        IReadOnlyDictionary<string, object>? query,
        CancellationToken cancellationToken,
        NavOptions? options = null)
    {
        if (!_routes.TryGetRoute(viewModelType, out var mapped))
        {
            return Task.FromResult(Result.Failure("E_ROUTE", $"No Shell route mapped for {viewModelType.Name}."));
        }

        var uri = mapped;
        if (query is { Count: > 0 })
        {
            uri = $"{mapped}?{NavigationRouteTable.FormatQuery(query)}";
        }
        else if (args is not null)
        {
            uri = $"{mapped}?{FormatQuery(args)}";
        }

        return GoUriAsync(uri, viewModelType, args, mapped, query, options, cancellationToken);
    }

    private async Task<Result> GoUriAsync(
        string uri,
        Type viewModelType,
        object? args,
        string? route,
        IReadOnlyDictionary<string, object>? query,
        NavOptions? options,
        CancellationToken cancellationToken,
        bool pop = false,
        bool reset = false)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var shell = Shell.Current;
        if (shell is null)
        {
            return Result.Failure("E_SHELL", "Shell.Current is not available.");
        }

        if (await IsBlockedAsync(cancellationToken).ConfigureAwait(true))
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

        var frame = new NavigationRequest(viewModelType, args, route, query, options?.Modal == true);
        if (pop)
        {
            _stack.Pop();
        }
        else if (reset)
        {
            _stack.Reset(frame);
        }
        else if (options?.Replace == true)
        {
            _stack.Replace(frame);
        }
        else
        {
            _stack.Push(frame, options);
        }

        return Result.Success();
    }

    private static async Task<bool> IsBlockedAsync(CancellationToken cancellationToken)
    {
        var page = MauiVisualTree.CurrentPage();
        return page?.BindingContext is INavigable navigable
            && !await navigable.CanNavigateAwayAsync(cancellationToken).ConfigureAwait(true);
    }

    private static string BuildUri(string path, IReadOnlyDictionary<string, object> query)
        => query.Count == 0 ? path : $"{path}?{NavigationRouteTable.FormatQuery(query)}";
}
