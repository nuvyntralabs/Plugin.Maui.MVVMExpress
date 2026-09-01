using Microsoft.Maui.Handlers;

namespace Plugin.Maui.MVVMExpress.Lifecycle;

/// <summary>Attaches <see cref="ViewModelLifecycleBehavior"/> to pages created by the MAUI handler.</summary>
public static class ViewModelLifecycleHost
{
    private static int _enabled;

    /// <summary>Registers a page-handler mapping once per process.</summary>
    public static void Enable(Hosting.MvvmExpressOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (Interlocked.Exchange(ref _enabled, 1) == 1)
        {
            return;
        }

        PageHandler.Mapper.AppendToMapping("MvvmExpressLifecycle", (_, view) =>
        {
            if (view is Page page)
            {
                Attach(page, options);
            }
        });
    }

    /// <summary>Adds the behavior when the page does not already have one.</summary>
    public static void Attach(Page page, Hosting.MvvmExpressOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(page);
        foreach (var behavior in page.Behaviors)
        {
            if (behavior is ViewModelLifecycleBehavior)
            {
                return;
            }
        }

        page.Behaviors.Add(new ViewModelLifecycleBehavior(options));
    }
}
