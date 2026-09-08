using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundDialogViewModel))]
public partial class DialogPage : ContentPage
{
    public DialogPage(PlaygroundDialogViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
