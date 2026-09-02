using Microsoft.Extensions.Logging;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Playground.Pages;
using Plugin.Maui.MVVMExpress.Samples;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMvvmExpress(o => o
                .UseNavigationPage((nav, _) => nav
                    .Map<PlaygroundHomeViewModel, HomePage>("home")
                    .Map<PlaygroundCommandViewModel, CommandPage>("command")
                    .Map<PlaygroundDetailsViewModel, DetailsPage>("details")
                    .Map<PlaygroundDialogViewModel, DialogPage>("dialog")
                    .Map<PlaygroundFormViewModel, FormPage>("form")
                    .Map<PlaygroundLoginViewModel, LoginPage>("login")
                    .Map<PlaygroundSecureViewModel, SecurePage>("secure")
                    .Map<PlaygroundListViewModel, ListPage>("list"))
                .UseDialogs()
                .UseAuth<PlaygroundLoginViewModel>())
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMvvmExpressSamples(configureNavigator: false);
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<CommandPage>();
        builder.Services.AddTransient<DetailsPage>();
        builder.Services.AddTransient<DialogPage>();
        builder.Services.AddTransient<FormPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SecurePage>();
        builder.Services.AddTransient<ListPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
