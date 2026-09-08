using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>Host <c>UseX()</c> adapters for sibling MauiEssentials plugins.</summary>
public static class MvvmExpressAdapterExtensions
{
    /// <summary>
    /// Maps incoming URIs onto <see cref="INavigator"/>. Pass <paramref name="bridge"/> from Plugin.Maui.DeepLinks.
    /// The parameterless call throws — it does not no-op.
    /// </summary>
    public static MvvmExpressOptions UseDeepLinks(this MvvmExpressOptions options, IDeepLinkBridge? bridge = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.AddRegistration(services =>
        {
            if (bridge is null)
            {
                services.AddDeepLinks();
                return;
            }

            services.AddDeepLinks(bridge);
        });
    }

    /// <summary>
    /// Registers a production <see cref="IAuthState"/>. Pass a SecureSession / SecureStorage factory.
    /// The parameterless call throws — it does not no-op.
    /// </summary>
    public static MvvmExpressOptions UseSecureSessionAuth(
        this MvvmExpressOptions options,
        Func<IServiceProvider, IAuthState>? factory = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.AddRegistration(services =>
        {
            if (factory is null)
            {
                services.AddSecureSessionAuth();
                return;
            }

            services.AddSecureSessionAuth(factory);
        });
    }
}
