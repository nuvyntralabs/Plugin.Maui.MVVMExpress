using Microsoft.Extensions.DependencyInjection.Extensions;
using Plugin.Maui.MVVMExpress.Hosting;

namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>Host option to install <see cref="MauiDialogs"/> and <see cref="MauiNotifier"/>.</summary>
public static class MvvmExpressDialogsExtensions
{
    /// <summary>Replaces <see cref="IDialogs"/> / <see cref="INotifier"/> with MAUI adapters.</summary>
    public static MvvmExpressOptions UseDialogs(this MvvmExpressOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.AddRegistration(static services =>
        {
            services.RemoveAll<IDialogs>();
            services.RemoveAll<INotifier>();
            services.AddSingleton<IDialogs, MauiDialogs>();
            services.AddSingleton<INotifier, MauiNotifier>();
        });
    }
}
