using System.Diagnostics.CodeAnalysis;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>
/// <see cref="IPageNavigator"/> that pushes pages onto <c>INavigation</c> / <c>NavigationPage</c>.
/// </summary>
public sealed class MauiPageNavigator : IPageNavigator, IRouteResolver
{
    private readonly Dictionary<Type, Type> _pages = [];
    private readonly NavigationRouteTable _routes = new();
    private readonly NavigationStack _stack = new();
    private readonly IServiceProvider? _services;
    private readonly Func<INavigation?>? _navigation;

    /// <summary>Creates a page-stack navigator for <paramref name="window"/>.</summary>
    /// <param name="window">Window this stack belongs to.</param>
    /// <param name="services">Optional DI used to construct pages and ViewModels.</param>
    /// <param name="navigation">Optional <see cref="INavigation"/> resolver; defaults to the window's current page.</param>
    public MauiPageNavigator(
        IWindowContext? window = null,
        IServiceProvider? services = null,
        Func<INavigation?>? navigation = null)
    {
        Window = window ?? WindowContext.Default;
        _services = services;
        _navigation = navigation;
    }

    /// <inheritdoc />
    public IWindowContext Window { get; }

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

    /// <summary>Maps <typeparamref name="TViewModel"/> to <typeparamref name="TPage"/> and an optional URI route.</summary>
    public MauiPageNavigator Map<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TPage>(string? route = null)
        where TViewModel : class, IViewModel
        where TPage : Page
    {
        _pages[typeof(TViewModel)] = typeof(TPage);
        _routes.Map<TViewModel>(string.IsNullOrWhiteSpace(route) ? typeof(TViewModel).Name : route);
        return this;
    }

    /// <inheritdoc />
    public bool TryResolve(string route, out Type viewModelType) => _routes.TryResolve(route, out viewModelType);

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => GoAsync(typeof(TViewModel), null, null, null, cancellationToken);

    /// <inheritdoc />
    public Task<Result> NavigateToAsync<TViewModel, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull
    {
        var query = args as IReadOnlyDictionary<string, object>;
        return GoAsync(typeof(TViewModel), args, null, query, cancellationToken);
    }

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
        if (!_routes.TryResolve(split.Path, out var viewModelType))
        {
            return Task.FromResult(Result.Failure("E_ROUTE", $"No page mapped for '{split.Path}'."));
        }

        _routes.TryGetRoute(viewModelType, out var mapped);
        return GoAsync(viewModelType, merged, mapped ?? split.Path, merged, cancellationToken, options);
    }

    /// <inheritdoc />
    public Task<Result> GoBackAsync(CancellationToken cancellationToken = default)
        => PopCore(toRoot: false, cancellationToken);

    /// <inheritdoc />
    public Task<Result> PopToRootAsync(CancellationToken cancellationToken = default)
        => PopCore(toRoot: true, cancellationToken);

    /// <inheritdoc />
    public Task<Result> ReplaceAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => GoAsync(typeof(TViewModel), null, null, null, cancellationToken, new NavOptions { Replace = true });

    /// <inheritdoc />
    public Task<Result> ResetAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => GoAsync(typeof(TViewModel), null, null, null, cancellationToken, reset: true);

    private async Task<Result> GoAsync(
        Type viewModelType,
        object? args,
        string? route,
        IReadOnlyDictionary<string, object>? query,
        CancellationToken cancellationToken,
        NavOptions? options = null,
        bool reset = false)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_pages.TryGetValue(viewModelType, out var pageType))
        {
            return Result.Failure("E_ROUTE", $"No page mapped for {viewModelType.Name}.");
        }

        var navigation = ResolveNavigation();
        if (navigation is null)
        {
            return Result.Failure("E_PAGE", "No INavigation host is available for this window.");
        }

        var currentPage = MauiVisualTree.CurrentPage(Window);
        if (currentPage?.BindingContext is INavigable leaving
            && !await leaving.CanNavigateAwayAsync(cancellationToken).ConfigureAwait(true))
        {
            return Result.Failure("E_GUARD", "Navigation blocked");
        }

        Page page;
        try
        {
            page = CreatePage(pageType, viewModelType, args, query);
        }
        catch (Exception ex)
        {
            return Result.Failure("E_PAGE", ex.Message, ex);
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (reset)
                {
                    await navigation.PopToRootAsync(options?.Animated ?? false).ConfigureAwait(true);
                    await navigation.PushAsync(page, options?.Animated ?? true).ConfigureAwait(true);
                    return;
                }

                if (options?.Replace == true && navigation.NavigationStack.Count > 1)
                {
                    await navigation.PopAsync(false).ConfigureAwait(true);
                }

                if (options?.Modal == true)
                {
                    await navigation.PushModalAsync(page, options.Animated).ConfigureAwait(true);
                }
                else
                {
                    await navigation.PushAsync(page, options?.Animated ?? true).ConfigureAwait(true);
                }
            }).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            return Result.Failure("E_NAV", ex.Message, ex);
        }

        if (currentPage?.BindingContext is INavigable from)
        {
            await from.OnNavigatedFromAsync(cancellationToken).ConfigureAwait(true);
        }

        if (page.BindingContext is INavigable to)
        {
            await to.OnNavigatedToAsync(cancellationToken).ConfigureAwait(true);
        }

        var frame = new NavigationRequest(viewModelType, args, route, query, options?.Modal == true);
        if (reset)
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

    private async Task<Result> PopCore(bool toRoot, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var navigation = ResolveNavigation();
        if (navigation is null)
        {
            return Result.Failure("E_PAGE", "No INavigation host is available for this window.");
        }

        var currentPage = MauiVisualTree.CurrentPage(Window);
        if (currentPage?.BindingContext is INavigable leaving
            && !await leaving.CanNavigateAwayAsync(cancellationToken).ConfigureAwait(true))
        {
            return Result.Failure("E_GUARD", "Navigation blocked");
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (toRoot)
                {
                    await navigation.PopToRootAsync().ConfigureAwait(true);
                }
                else if (navigation.ModalStack.Count > 0)
                {
                    await navigation.PopModalAsync().ConfigureAwait(true);
                }
                else
                {
                    await navigation.PopAsync().ConfigureAwait(true);
                }
            }).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            return Result.Failure("E_NAV", ex.Message, ex);
        }

        if (currentPage?.BindingContext is INavigable from)
        {
            await from.OnNavigatedFromAsync(cancellationToken).ConfigureAwait(true);
        }

        if (toRoot)
        {
            _stack.PopToRoot();
        }
        else
        {
            _stack.Pop();
        }

        return Result.Success();
    }

    private INavigation? ResolveNavigation() => _navigation is not null ? _navigation() : MauiVisualTree.CurrentNavigation(Window);

    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Map<TViewModel,TPage> captures constructors; DI is the AOT path.")]
    private Page CreatePage(
        Type pageType,
        Type viewModelType,
        object? args,
        IReadOnlyDictionary<string, object>? query)
    {
        var page = _services?.GetService(pageType) as Page
            ?? (Page)Activator.CreateInstance(pageType)!;

        if (page.BindingContext is null && _services?.GetService(viewModelType) is { } viewModel)
        {
            page.BindingContext = viewModel;
        }

        NavArgsApplier.ApplyQuery(page.BindingContext, query);
        if (args is not null && args is not IReadOnlyDictionary<string, object>)
        {
            ApplyTypedArgs(page.BindingContext, args);
        }

        return page;
    }

    private static void ApplyTypedArgs(object? viewModel, object args)
    {
        if (viewModel is null)
        {
            return;
        }

        foreach (var iface in viewModel.GetType().GetInterfaces())
        {
            if (!iface.IsGenericType || iface.GetGenericTypeDefinition() != typeof(IAcceptNavArgs<>))
            {
                continue;
            }

            if (!iface.GenericTypeArguments[0].IsInstanceOfType(args))
            {
                continue;
            }

            iface.GetMethod(nameof(IAcceptNavArgs<object>.Accept))?.Invoke(viewModel, [args]);
            return;
        }
    }
}
