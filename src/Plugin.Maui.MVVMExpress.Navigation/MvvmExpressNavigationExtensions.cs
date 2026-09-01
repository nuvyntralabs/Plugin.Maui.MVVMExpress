using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.Diagnostics;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>Host options to install Shell or <see cref="NavigationPage"/> navigators.</summary>
public static class MvvmExpressNavigationExtensions
{
    /// <summary>Replaces <see cref="INavigator"/> with <see cref="MauiShellNavigator"/>. Optional — not the default.</summary>
    /// <param name="options">Host options.</param>
    /// <param name="configure">Optional mapping callback.</param>
    public static MvvmExpressOptions UseShell(
        this MvvmExpressOptions options,
        Action<MauiShellNavigator, IServiceProvider>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.AddRegistration(services =>
        {
            services.RemoveAll<INavigator>();
            services.AddSingleton<INavigator>(sp =>
            {
                var navigator = new MauiShellNavigator(
                    services,
                    sp.GetService<IMainThread>(),
                    sp.GetService<IMvvmExpressDiagnostics>());
                configure?.Invoke(navigator, sp);
                return navigator;
            });
        });
    }

    /// <summary>
    /// Registers <see cref="MauiPageNavigator"/> as <see cref="INavigator"/> / <see cref="IPageNavigator"/>.
    /// Use this for login → replace-root → push thread. Shell is not required.
    /// </summary>
    /// <param name="options">Host options.</param>
    /// <param name="configure">Optional mapping callback.</param>
    public static MvvmExpressOptions UseNavigationPage(
        this MvvmExpressOptions options,
        Action<MauiPageNavigator, IServiceProvider>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.AddRegistration(services =>
        {
            services.RemoveAll<INavigator>();
            services.RemoveAll<IPageNavigator>();
            services.AddSingleton<MauiPageNavigator>(sp =>
            {
                var window = sp.GetService<IWindowContext>();
                var navigator = new MauiPageNavigator(
                    window,
                    sp,
                    () => MauiVisualTree.CurrentNavigation(window),
                    sp.GetService<IMainThread>(),
                    sp.GetService<IMvvmExpressDiagnostics>(),
                    static () => Application.Current is { Windows.Count: > 0 } app ? app.Windows[0] : null);
                configure?.Invoke(navigator, sp);
                return navigator;
            });
            services.AddSingleton<IPageNavigator>(sp => sp.GetRequiredService<MauiPageNavigator>());
            services.AddSingleton<INavigator>(sp => sp.GetRequiredService<MauiPageNavigator>());
        });
    }
}
