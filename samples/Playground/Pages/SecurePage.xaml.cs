using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

public partial class SecurePage : ContentPage
{
    public SecurePage(PlaygroundSecureViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
