using Plugin.Maui.MVVMExpress.Samples.Navigation;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class HomePage : SampleContentPage
{
    public HomePage(HomeViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
