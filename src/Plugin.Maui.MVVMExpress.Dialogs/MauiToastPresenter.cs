using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>Overlays a toast on the current window without rewriting page content.</summary>
public sealed class MauiToastPresenter : IToastPresenter
{
    private readonly IWindowContext _window;

    /// <summary>Creates a presenter for <paramref name="window"/>.</summary>
    public MauiToastPresenter(IWindowContext? window = null)
        => _window = window ?? WindowContext.Default;

    /// <inheritdoc />
    public async Task ShowAsync(string message, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        cancellationToken.ThrowIfCancellationRequested();
        if (duration <= TimeSpan.Zero)
        {
            duration = TimeSpan.FromSeconds(2);
        }

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var page = MauiVisualTree.CurrentPage(_window)
                ?? throw new FeatureNotSupportedException("A page is required to show a toast.");

            using (MauiToastOverlay.Show(page, message))
            {
                await Task.Delay(duration, cancellationToken).ConfigureAwait(true);
            }
        }).ConfigureAwait(true);
    }
}
