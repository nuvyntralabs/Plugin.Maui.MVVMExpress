using Plugin.Maui.MVVMExpress.ComponentModel;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>ViewModel navigation. Implementations must not require a <c>Page</c> reference on the ViewModel.</summary>
public interface INavigator
{
    /// <summary>Last navigated ViewModel type, if any.</summary>
    Type? Current { get; }

    /// <summary>Recorded navigation requests (including back).</summary>
    IReadOnlyList<NavigationRequest> History { get; }

    /// <summary>Navigates to <typeparamref name="TViewModel"/>.</summary>
    Task<Result> NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel;

    /// <summary>Navigates to <typeparamref name="TViewModel"/> with typed <paramref name="args"/>.</summary>
    Task<Result> NavigateToAsync<TViewModel, TArgs>(TArgs args, CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        where TArgs : notnull;

    /// <summary>Pops one level.</summary>
    Task<Result> GoBackAsync(CancellationToken cancellationToken = default);
}

/// <summary>One recorded navigation.</summary>
/// <param name="ViewModelType">Destination ViewModel type.</param>
/// <param name="Args">Typed args or a sentinel such as <c>back</c>.</param>
public sealed record NavigationRequest(Type ViewModelType, object? Args);

/// <summary>Destination that accepts typed navigation arguments.</summary>
/// <typeparam name="TArgs">Argument type.</typeparam>
public interface IAcceptNavArgs<TArgs>
    where TArgs : notnull
{
    /// <summary>Applies <paramref name="args"/> before initialize/load.</summary>
    void Accept(TArgs args);
}

/// <summary>Navigation lifecycle used by <see cref="PageViewModel"/>.</summary>
public interface INavigable
{
    /// <summary>Called after the host has navigated to this ViewModel.</summary>
    Task OnNavigatedToAsync(CancellationToken cancellationToken = default);

    /// <summary>Called before the host leaves this ViewModel.</summary>
    Task OnNavigatedFromAsync(CancellationToken cancellationToken = default);

    /// <summary>Return <see langword="false"/> to block navigation away (dirty form, etc.).</summary>
    Task<bool> CanNavigateAwayAsync(CancellationToken cancellationToken = default);
}
