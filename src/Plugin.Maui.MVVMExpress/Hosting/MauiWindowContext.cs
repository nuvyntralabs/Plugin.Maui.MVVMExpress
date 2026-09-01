using System.Runtime.CompilerServices;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>Maps a MAUI <see cref="Window"/> to an <see cref="IWindowContext"/>.</summary>
public static class MauiWindowContext
{
    private static readonly ConditionalWeakTable<Window, WindowContext> Map = [];
    private static int _next;

    /// <summary>Returns a stable context for <paramref name="window"/>.</summary>
    public static IWindowContext For(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        return Map.GetValue(window, static _ => new WindowContext($"window-{Interlocked.Increment(ref _next)}"));
    }

    /// <summary>Context for the first application window, or <see cref="WindowContext.Default"/>.</summary>
    public static IWindowContext Current
    {
        get
        {
            var app = Application.Current;
            if (app is { Windows.Count: > 0 })
            {
                return For(app.Windows[0]);
            }

            return WindowContext.Default;
        }
    }
}
