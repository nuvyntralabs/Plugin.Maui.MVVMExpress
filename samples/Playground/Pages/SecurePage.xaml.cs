using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundSecureViewModel))]
public partial class SecurePage : ContentPage
{
    public SecurePage(PlaygroundSecureViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
