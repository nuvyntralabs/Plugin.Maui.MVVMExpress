using Plugin.Maui.MVVMExpress.Samples.Playground;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

public partial class ListPage : ContentPage
{
    public ListPage(PlaygroundListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
