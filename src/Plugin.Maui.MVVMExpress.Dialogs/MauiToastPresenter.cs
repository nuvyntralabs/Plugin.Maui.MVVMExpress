using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>Overlays a toast on the current page of a window.</summary>
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
            var page = MauiVisualTree.CurrentPage(_window);
            if (page is not ContentPage content)
            {
                throw new FeatureNotSupportedException("A ContentPage is required to show a toast.");
            }

            var toast = CreateToast(message);
            var host = EnsureOverlayHost(content, out var wrappedOriginal);
            host.Add(toast);
            try
            {
                await Task.Delay(duration, cancellationToken).ConfigureAwait(true);
            }
            finally
            {
                host.Remove(toast);
                if (wrappedOriginal is not null && ReferenceEquals(content.Content, host))
                {
                    host.Remove(wrappedOriginal);
                    content.Content = wrappedOriginal;
                }
            }
        }).ConfigureAwait(true);
    }

    private static View CreateToast(string message)
        => new Border
        {
            BackgroundColor = Color.FromArgb("#CC323232"),
            StrokeThickness = 0,
            Padding = new Thickness(16, 10),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(16, 0, 16, 28),
            InputTransparent = true,
            Content = new Label
            {
                Text = message,
                TextColor = Colors.White,
                LineBreakMode = LineBreakMode.WordWrap
            }
        };

    private static Grid EnsureOverlayHost(ContentPage page, out View? wrappedOriginal)
    {
        if (page.Content is Grid grid)
        {
            wrappedOriginal = null;
            return grid;
        }

        var original = page.Content;
        var host = new Grid();
        if (original is not null)
        {
            host.Add(original);
        }

        page.Content = host;
        wrappedOriginal = original;
        return host;
    }
}
