using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Outcome;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>MAUI page alert adapter. Resolves the current page from the window and hops to <see cref="IMainThread"/>.</summary>
public sealed class MauiDialogs : IDialogs
{
    private readonly IWindowContext _window;
    private readonly IMainThread _main;

    /// <summary>Creates dialogs for <paramref name="window"/>.</summary>
    public MauiDialogs(IWindowContext? window = null, IMainThread? mainThread = null)
    {
        _window = window ?? WindowContext.Default;
        _main = mainThread ?? NotificationMarshaller.Current ?? ImmediateMainThread.Instance;
    }

    /// <inheritdoc />
    public Task AlertAsync(string title, string message, string cancel = "OK", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _main.InvokeAsync(() => CurrentPage().DisplayAlertAsync(title, message, cancel), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ConfirmAsync(string title, string message, string accept = "OK", string cancel = "Cancel", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var accepted = false;
        await _main.InvokeAsync(
            async () => accepted = await CurrentPage().DisplayAlertAsync(title, message, accept, cancel).ConfigureAwait(true),
            cancellationToken).ConfigureAwait(false);
        return accepted;
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
