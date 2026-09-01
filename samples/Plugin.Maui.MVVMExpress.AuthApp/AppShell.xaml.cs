using Plugin.Maui.MVVMExpress.AuthApp.Pages;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.AuthApp;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);
        InitializeComponent();
        Items.Add(MauiShellNavigator.CreateContent<LoginPage>("login", services));
        Items.Add(MauiShellNavigator.CreateContent<HomePage>("home", services));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("forgot", typeof(ForgotPage));
    }
}
