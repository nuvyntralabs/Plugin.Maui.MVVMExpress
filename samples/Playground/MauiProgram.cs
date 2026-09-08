using Microsoft.Extensions.Logging;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Playground.Pages;
using Plugin.Maui.MVVMExpress.Samples;
using Plugin.Maui.MVVMExpress.Samples.ChatHost;
using Plugin.Maui.MVVMExpress.Samples.Computed;
using Plugin.Maui.MVVMExpress.Samples.EscapeHatch;
using Plugin.Maui.MVVMExpress.Samples.Modals;
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
                    .Map<PlaygroundListViewModel, ListPage>("list")
                    .Map<ChatHostViewModel, ChatHostPage>("chats")
                    .Map<ComputedNameViewModel, ComputedNamePage>("computed")
                    .Map<ManualCounterViewModel, ManualCounterPage>("manual")
                    .Map<ModalHostViewModel, ModalHostPage>("modal"))
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
        builder.Services.AddTransient<ChatHostPage>();
        builder.Services.AddTransient<ComputedNamePage>();
        builder.Services.AddTransient<ManualCounterPage>();
        builder.Services.AddTransient<ModalHostPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
