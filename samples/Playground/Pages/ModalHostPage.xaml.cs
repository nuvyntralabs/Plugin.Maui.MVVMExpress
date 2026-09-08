using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Modals;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(ModalHostViewModel))]
public partial class ModalHostPage : ContentPage
{
    public ModalHostPage(ModalHostViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
