using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.AuthApp;

namespace Plugin.Maui.MVVMExpress.AuthApp.Pages;

[RegisterView(typeof(AuthHomeViewModel))]
public partial class HomePage : ContentPage
{
    public HomePage(AuthHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
