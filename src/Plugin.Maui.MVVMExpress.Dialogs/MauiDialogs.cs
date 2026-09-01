using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Outcome;

namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>MAUI page alert adapter. Resolves the current page from the window.</summary>
public sealed class MauiDialogs : IDialogs
{
    private readonly IWindowContext _window;

    /// <summary>Creates dialogs for <paramref name="window"/>.</summary>
    public MauiDialogs(IWindowContext? window = null)
        => _window = window ?? WindowContext.Default;

    /// <inheritdoc />
    public Task AlertAsync(string title, string message, string cancel = "OK", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return CurrentPage().DisplayAlertAsync(title, message, cancel);
    }

    /// <inheritdoc />
    public Task<bool> ConfirmAsync(string title, string message, string accept = "OK", string cancel = "Cancel", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return CurrentPage().DisplayAlertAsync(title, message, accept, cancel);
    }

    /// <inheritdoc />
    public Task ErrorAsync(ErrorInfo error, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(error);
        return AlertAsync("Error", error.Message, cancellationToken: cancellationToken);
    }

    private Page CurrentPage()
        => MauiVisualTree.CurrentPage(_window)
            ?? throw new InvalidOperationException("No current page for this window.");
}
