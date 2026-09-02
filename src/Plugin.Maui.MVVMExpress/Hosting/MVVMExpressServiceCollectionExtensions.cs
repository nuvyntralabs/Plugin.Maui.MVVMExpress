using Microsoft.Extensions.DependencyInjection.Extensions;
#if DEBUG
using Plugin.Maui.MVVMExpress.Diagnostics;
#endif
using Plugin.Maui.MVVMExpress.Generated;
using Plugin.Maui.MVVMExpress.Lifecycle;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>MAUI builder entry point.</summary>
public static class MVVMExpressMauiAppBuilderExtensions
{
    /// <summary>Registers MVVMExpress and the MAUI dispatcher.</summary>
    /// <param name="builder">MAUI builder.</param>
    /// <param name="configure">Optional options.</param>
    public static MauiAppBuilder UseMvvmExpress(
        this MauiAppBuilder builder,
        Action<MvvmExpressOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var options = new MvvmExpressOptions();
        configure?.Invoke(options);
        builder.Services.AddSingleton(options);
        builder.Services.AddMvvmExpress();
        var main = new MauiMainThread();
        builder.Services.RemoveAll<IMainThread>();
        builder.Services.AddSingleton<IMainThread>(main);
        NotificationMarshaller.Current = main;
        NotificationMarshaller.MarshalNotifications = options.MarshalNotifications;
        builder.Services.RemoveAll<IWindowContext>();
        builder.Services.AddSingleton<IWindowContext>(_ => MauiWindowContext.Current);
#if DEBUG
        if (options.EnableDiagnostics)
        {
            var diagnostics = new CallbackDiagnostics(static (area, message) =>
                System.Diagnostics.Debug.WriteLine($"[MVVMExpress:{area}] {message}"));
            builder.Services.RemoveAll<IMvvmExpressDiagnostics>();
            builder.Services.AddSingleton<IMvvmExpressDiagnostics>(_ => diagnostics);
            NotificationMarshaller.Diagnostics = diagnostics;
        }
#endif
        options.ApplyRegistrations(builder.Services);
        if (options.ApplyGeneratedRegistrations)
        {
            GeneratedRegistrationHooks.Apply(builder.Services);
        }

        if (options.AuthChallengeViewModel is { } challenge)
        {
            builder.Services.AddAuth(challenge, options.ForwardNavigationFailures);
        }

        if (options.AutoAttachLifecycle)
        {
            ViewModelLifecycleHost.Enable(options);
        }

        return builder;
    }
}
