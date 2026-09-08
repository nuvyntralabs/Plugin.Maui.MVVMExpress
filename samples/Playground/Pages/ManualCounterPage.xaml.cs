using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.EscapeHatch;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(ManualCounterViewModel))]
public partial class ManualCounterPage : ContentPage
{
    public ManualCounterPage(ManualCounterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
