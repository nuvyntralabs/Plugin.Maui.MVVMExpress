using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.Hosting;

namespace Plugin.Maui.MVVMExpress.Navigation;

/// <summary>Host option to install <see cref="MauiShellNavigator"/>.</summary>
public static class MvvmExpressNavigationExtensions
{
    /// <summary>Replaces <see cref="INavigator"/> with <see cref="MauiShellNavigator"/>.</summary>
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
                var navigator = new MauiShellNavigator(services);
                configure?.Invoke(navigator, sp);
                return navigator;
            });
        });
    }
}
