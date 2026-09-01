using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>Resolves the current <see cref="Page"/> for a window.</summary>
public static class MauiVisualTree
{
    /// <summary>Returns the visible page for <paramref name="window"/>, or the first app window.</summary>
    public static Page? CurrentPage(IWindowContext? window = null)
    {
        if (Shell.Current?.CurrentPage is { } shellPage
            && (window is null || Matches(Shell.Current.Window, window)))
        {
            return shellPage;
        }

        var app = Application.Current;
        if (app is null)
        {
            return null;
        }

        if (window is not null)
        {
            foreach (var candidate in app.Windows)
            {
                if (MauiWindowContext.For(candidate).WindowId == window.WindowId)
                {
                    return Unwrap(candidate.Page);
                }
            }
        }

        return app.Windows.Count > 0 ? Unwrap(app.Windows[0].Page) : null;
    }

    /// <summary>Returns <see cref="INavigation"/> for the current page of <paramref name="window"/>.</summary>
    public static INavigation? CurrentNavigation(IWindowContext? window = null)
        => CurrentPage(window)?.Navigation;

    private static Page? Unwrap(Page? page) => page switch
    {
        Shell shell => shell.CurrentPage ?? shell,
        NavigationPage navigation => navigation.CurrentPage ?? navigation,
        _ => page
    };

    private static bool Matches(Window? mauiWindow, IWindowContext window)
        => mauiWindow is not null && MauiWindowContext.For(mauiWindow).WindowId == window.WindowId;
}
