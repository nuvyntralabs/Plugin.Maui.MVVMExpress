using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

public partial class HomePage : ContentPage
{
    public HomePage(AuthHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
