using Microsoft.Extensions.DependencyInjection.Extensions;
#if DEBUG
using Plugin.Maui.MVVMExpress.Diagnostics;
#endif
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
        builder.Services.RemoveAll<IMainThread>();
        builder.Services.AddSingleton<IMainThread, MauiMainThread>();
#if DEBUG
        if (options.EnableDiagnostics)
        {
            builder.Services.RemoveAll<IMvvmExpressDiagnostics>();
            builder.Services.AddSingleton<IMvvmExpressDiagnostics>(_ =>
                new CallbackDiagnostics(static (area, message) =>
                    System.Diagnostics.Debug.WriteLine($"[MVVMExpress:{area}] {message}")));
        }
#endif
        return builder;
    }
}
