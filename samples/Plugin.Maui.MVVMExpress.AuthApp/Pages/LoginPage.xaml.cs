using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

[RegisterView(typeof(AuthLoginViewModel))]
public partial class LoginPage : ContentPage
{
    public LoginPage(AuthLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
