using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

public partial class CommandPage : ContentPage
{
    public CommandPage(PlaygroundCommandViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
