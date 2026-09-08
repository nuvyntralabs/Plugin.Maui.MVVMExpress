using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundHomeViewModel))]
public partial class HomePage : ContentPage
{
    public HomePage(PlaygroundHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
