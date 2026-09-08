using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundFormViewModel))]
public partial class FormPage : ContentPage
{
    public FormPage(PlaygroundFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
