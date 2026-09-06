using Microsoft.Extensions.Logging;
using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMvvmExpress(o => o
                .UseNavigationPage((nav, _) => nav
                    .Map<MainPageViewModel, MainPage>("main")
                    .Map<LoginViewModel, LoginPage>("login")
                    .Map<ItemListViewModel, ItemListPage>("list")
                    .Map<ItemEditViewModel, ItemEditPage>("edit"))
                .UseDialogs()
                .UseAuth<LoginViewModel>())
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IAuthState, DemoAuthState>();
        builder.Services.AddSingleton<IItemStore, MemoryItemStore>();
        builder.Services.AddMain();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ItemListPage>();
        builder.Services.AddTransient<ItemEditPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
