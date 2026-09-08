using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

[RegisterView(typeof(AuthRegisterViewModel))]
public partial class RegisterPage : ContentPage
{
    public RegisterPage(AuthRegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
