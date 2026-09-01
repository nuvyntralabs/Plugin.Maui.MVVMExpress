using Microsoft.Extensions.Logging;
using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.AuthApp.Pages;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples;
using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMvvmExpress(o => o.UseShell().UseDialogs())
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMvvmExpressSamples();
        builder.Services.AddSingleton<INavigator>(sp =>
        {
            var shell = new MauiShellNavigator(builder.Services)
                .Map<AuthLoginViewModel, LoginPage>("//login")
                .Map<AuthRegisterViewModel, RegisterPage>("register")
                .Map<AuthForgotViewModel, ForgotPage>("forgot")
                .Map<AuthHomeViewModel, HomePage>("//home");
            return new GuardedNavigator(
                shell,
                sp.GetRequiredService<IAuthState>(),
                Plugin.Maui.MVVMExpress.Generated.MvvmExpressGeneratedRegistrations.AuthPolicy,
                new GuardedNavigatorOptions { ChallengeViewModel = typeof(AuthLoginViewModel), ForwardFailures = true },
                typeof(AuthHomeViewModel));
        });
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ForgotPage>();
        builder.Services.AddTransient<HomePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
