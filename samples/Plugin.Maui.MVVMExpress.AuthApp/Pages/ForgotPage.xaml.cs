using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

[RegisterView(typeof(AuthForgotViewModel))]
public partial class ForgotPage : ContentPage
{
    public ForgotPage(AuthForgotViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
