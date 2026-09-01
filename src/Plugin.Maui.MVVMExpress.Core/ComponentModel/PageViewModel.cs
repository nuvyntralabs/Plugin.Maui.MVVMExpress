using Plugin.Maui.MVVMExpress.Busy;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.ComponentModel;

/// <summary>Page-scoped ViewModel with navigator/dialogs and navigation guards.</summary>
public abstract class PageViewModel : ViewModel, INavigable
{
    /// <summary>Creates a page ViewModel.</summary>
    /// <param name="navigator">Optional navigator.</param>
    /// <param name="dialogs">Optional dialogs.</param>
    /// <param name="errors">Optional unexpected-error sink.</param>
    /// <param name="busy">Optional nested busy gate.</param>
    protected PageViewModel(
        INavigator? navigator = null,
        IDialogs? dialogs = null,
        IErrorSink? errors = null,
        IBusyGate? busy = null)
        : base(errors, busy)
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
