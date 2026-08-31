using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.Busy;
using Plugin.Maui.MVVMExpress.Caching;
using Plugin.Maui.MVVMExpress.Connectivity;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Messaging;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>Registers Core services for tests, samples, and <c>UseMvvmExpress</c>.</summary>
public static class MVVMExpressServiceCollectionExtensions
{
    /// <summary>Adds Core singletons used by ViewModels.</summary>
    /// <param name="services">Service collection.</param>
    public static IServiceCollection AddMvvmExpress(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<IMessageHub, MessageHub>();
        services.TryAddSingleton<IBusyGate, BusyGate>();
        services.TryAddSingleton<IErrorSink, NullErrorSink>();
        services.TryAddSingleton<ICache, MemoryCache>();
        services.TryAddSingleton<IConnectivityProbe, InMemoryConnectivityProbe>();
        services.TryAddSingleton<INavigator, InMemoryNavigator>();
        services.TryAddSingleton<IMainThread>(_ => ImmediateMainThread.Instance);
        services.TryAddSingleton<IDialogs, NullDialogs>();
        return services;
    }
}
