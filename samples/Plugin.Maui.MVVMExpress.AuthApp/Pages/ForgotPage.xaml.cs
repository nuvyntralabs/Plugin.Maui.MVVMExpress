using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

public partial class ForgotPage : ContentPage
{
    public ForgotPage(AuthForgotViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
