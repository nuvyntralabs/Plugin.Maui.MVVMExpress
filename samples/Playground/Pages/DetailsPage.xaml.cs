using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundDetailsViewModel))]
public partial class DetailsPage : ContentPage
{
    public DetailsPage(PlaygroundDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
