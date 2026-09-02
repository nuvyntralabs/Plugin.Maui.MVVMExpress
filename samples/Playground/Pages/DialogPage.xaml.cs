using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

public partial class DialogPage : ContentPage
{
    public DialogPage(PlaygroundDialogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
