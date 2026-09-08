using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Navigation;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(HomeViewModel))]
public partial class HomePage : SampleContentPage
{
    public HomePage(HomeViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
