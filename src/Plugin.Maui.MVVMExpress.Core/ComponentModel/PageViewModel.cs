using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.ComponentModel;

/// <summary>Page-scoped ViewModel with navigator/dialogs and navigation guards.</summary>
public abstract class PageViewModel : ViewModel, INavigable
{
    /// <summary>Creates a page ViewModel.</summary>
    /// <param name="navigator">Optional navigator.</param>
    /// <param name="dialogs">Optional dialogs.</param>
    protected PageViewModel(INavigator? navigator = null, IDialogs? dialogs = null)
    {
        Navigator = navigator;
        Dialogs = dialogs;
    }

    /// <summary>Typed navigation service.</summary>
    protected INavigator? Navigator { get; }

    /// <summary>ViewModel-facing dialogs.</summary>
    protected IDialogs? Dialogs { get; }

    /// <inheritdoc />
    public virtual Task OnNavigatedToAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public virtual Task OnNavigatedFromAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public virtual Task<bool> CanNavigateAwayAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(true);
}
