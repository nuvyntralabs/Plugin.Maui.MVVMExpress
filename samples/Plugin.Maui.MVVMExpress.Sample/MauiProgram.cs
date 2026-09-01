using Microsoft.Extensions.Logging;
using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Sample.Pages;
using Plugin.Maui.MVVMExpress.Samples;
using Plugin.Maui.MVVMExpress.Samples.Auth;
using Plugin.Maui.MVVMExpress.Samples.Crud;
using Plugin.Maui.MVVMExpress.Samples.Enterprise;
using Plugin.Maui.MVVMExpress.Samples.Navigation;

namespace Plugin.Maui.MVVMExpress.Sample;

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
                .Map<ProductListViewModel>("//products")
                .Map<ProductDetailsViewModel>("details")
                .Map<SecureHomeViewModel>("secure");
            return new GuardedNavigator(
                shell,
                sp.GetRequiredService<IAuthState>(),
                Plugin.Maui.MVVMExpress.Generated.MvvmExpressGeneratedRegistrations.AuthPolicy,
                typeof(SecureHomeViewModel),
                typeof(EnterpriseShellViewModel));
        });
        builder.Services.AddSingleton<IPageNavigator>(sp => new MauiPageNavigator(
                new WindowContext("page-stack"),
                sp,
                () => Shell.Current?.CurrentPage?.Navigation)
            .Map<PageStackViewModel, PageStackPage>("stack")
            .Map<PageStackItemViewModel, PageStackItemPage>("stack-item"));
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<CounterPage>();
        builder.Services.AddTransient<ProductListPage>();
        builder.Services.AddTransient<ProductEditPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<PageStackPage>();
        builder.Services.AddTransient<PageStackItemPage>();
        builder.Services.AddTransient<ProductDetailsPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<SecureHomePage>();
        builder.Services.AddTransient<OfflinePage>();
        builder.Services.AddTransient<PaginationPage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<EnterprisePage>();
        builder.Services.AddTransient<ScopesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
