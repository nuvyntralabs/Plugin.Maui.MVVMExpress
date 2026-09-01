namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>
/// Draws a toast on <see cref="Window.AddOverlay"/> so page <c>Content</c> is never wrapped or replaced.
/// </summary>
internal static class MauiToastOverlay
{
    public static IDisposable Show(Page page, string message)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        if (page.Window is not Window window)
        {
            throw new FeatureNotSupportedException("A window is required to show a toast.");
        }

        var overlay = new DrawableToastOverlay(window, message);
        if (!window.AddOverlay(overlay))
        {
            throw new InvalidOperationException("The window rejected the toast overlay.");
        }

        return new Lease(window, overlay);
    }

    private sealed class Lease(Window window, IWindowOverlay overlay) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            window.RemoveOverlay(overlay);
            _disposed = true;
        }
    }

    private sealed class DrawableToastOverlay : WindowOverlay
    {
        public DrawableToastOverlay(IWindow window, string message)
            : base(window)
        {
            AddWindowElement(new ToastElement(message));
            EnableDrawableTouchHandling = false;
            DisableUITouchEventPassthrough = false;
            IsVisible = true;
        }
    }

    private sealed class ToastElement(string message) : IWindowOverlayElement
    {
        private const float HorizontalMargin = 16f;
        private const float BottomMargin = 28f;
        private const float PaddingX = 16f;
        private const float PaddingY = 10f;
        private const float FontSize = 14f;
        private const float CornerRadius = 8f;
        private const float MaxWidth = 360f;

        public bool Contains(Point point) => false;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var textSize = canvas.GetStringSize(message, Microsoft.Maui.Graphics.Font.Default, FontSize);
            var width = Math.Min(
                dirtyRect.Width - (HorizontalMargin * 2f),
                Math.Min(MaxWidth, Math.Max(textSize.Width + (PaddingX * 2f), 80f)));
            var height = textSize.Height + (PaddingY * 2f);
            var bounds = new RectF(
                (dirtyRect.Width - width) / 2f,
                dirtyRect.Height - height - BottomMargin,
                width,
                height);

            canvas.FillColor = Color.FromArgb("#CC323232");
            canvas.FillRoundedRectangle(bounds, CornerRadius);
            canvas.FontColor = Colors.White;
            canvas.FontSize = FontSize;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.DrawString(
                message,
                bounds,
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                TextFlow.ClipBounds);
        }
    }
}
