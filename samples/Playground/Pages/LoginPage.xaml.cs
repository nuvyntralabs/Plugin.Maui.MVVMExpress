using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(PlaygroundLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
