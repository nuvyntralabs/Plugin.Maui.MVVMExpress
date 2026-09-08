using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>
/// Optional sibling-plugin adapters. Core never takes a PackageReference to DeepLinks or SecureSession.
/// A missing adapter throws — it does not no-op.
/// </summary>
public static class SiblingAdapterExtensions
{
    /// <summary>
    /// Registers <paramref name="bridge"/>. Call the parameterless overload to fail closed when
    /// Plugin.Maui.DeepLinks is not wired.
    /// </summary>
    public static IServiceCollection AddDeepLinks(this IServiceCollection services, IDeepLinkBridge bridge)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(bridge);
        services.TryAddSingleton(bridge);
        return services;
    }

    /// <summary>Fail-closed DeepLinks registration when no adapter is supplied.</summary>
    public static IServiceCollection AddDeepLinks(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        throw new InvalidOperationException(
            "UseDeepLinks requires Plugin.Maui.DeepLinks. Register IDeepLinkBridge with AddDeepLinks(bridge) or UseDeepLinks(bridge).");
    }

    /// <summary>Registers a production <see cref="IAuthState"/> factory (SecureSession or SecureStorage adapter).</summary>
    public static IServiceCollection AddSecureSessionAuth(
        this IServiceCollection services,
        Func<IServiceProvider, IAuthState> factory)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(factory);
        services.RemoveAll<IAuthState>();
        services.AddSingleton(factory);
        return services;
    }

    /// <summary>Fail-closed SecureSession registration when no adapter is supplied.</summary>
    public static IServiceCollection AddSecureSessionAuth(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        throw new InvalidOperationException(
            "UseSecureSessionAuth requires Plugin.Maui.SecureSession (or MAUI SecureStorage). Register IAuthState with AddSecureSessionAuth(factory) or UseSecureSessionAuth(factory).");
    }
}
