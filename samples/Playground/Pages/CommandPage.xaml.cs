using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundCommandViewModel))]
public partial class CommandPage : ContentPage
{
    public CommandPage(PlaygroundCommandViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
