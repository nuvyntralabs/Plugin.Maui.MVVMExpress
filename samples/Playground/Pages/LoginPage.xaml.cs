using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundLoginViewModel))]
public partial class LoginPage : ContentPage
{
    public LoginPage(PlaygroundLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
