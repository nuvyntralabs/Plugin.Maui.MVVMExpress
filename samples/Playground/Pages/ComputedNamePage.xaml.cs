using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Computed;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(ComputedNameViewModel))]
public partial class ComputedNamePage : ContentPage
{
    public ComputedNamePage(ComputedNameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
