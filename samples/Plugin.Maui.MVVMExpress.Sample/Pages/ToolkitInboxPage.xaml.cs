using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Sample.Compatibility;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(ToolkitInboxViewModel))]
public partial class ToolkitInboxPage : ContentPage
{
    public ToolkitInboxPage(ToolkitInboxViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
