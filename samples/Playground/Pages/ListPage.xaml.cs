using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(PlaygroundListViewModel))]
public partial class ListPage : ContentPage
{
    public ListPage(PlaygroundListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
