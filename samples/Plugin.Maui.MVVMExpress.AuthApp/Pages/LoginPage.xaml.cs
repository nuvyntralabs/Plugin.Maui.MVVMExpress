using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(AuthLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
