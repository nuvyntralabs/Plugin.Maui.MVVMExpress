using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(AuthRegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
