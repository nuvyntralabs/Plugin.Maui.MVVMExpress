using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

public partial class HomePage : ContentPage
{
    public HomePage(PlaygroundHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
