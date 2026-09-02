using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

public partial class FormPage : ContentPage
{
    public FormPage(PlaygroundFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
