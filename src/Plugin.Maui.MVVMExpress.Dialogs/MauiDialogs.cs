using Plugin.Maui.MVVMExpress.Outcome;

namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>MAUI page alert adapter. Resolves the current page from the app.</summary>
public sealed class MauiDialogs : IDialogs
{
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

    private static Page CurrentPage()
    {
        if (Shell.Current?.CurrentPage is { } shellPage)
        {
            return shellPage;
        }

        return Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[0].Page
                ?? throw new InvalidOperationException("No current page.")
            : throw new InvalidOperationException("No current window.");
    }
}
