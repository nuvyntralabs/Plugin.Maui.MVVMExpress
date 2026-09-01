using Plugin.Maui.MVVMExpress.Samples.Auth;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class SecureHomePage : SampleContentPage
{
    public SecureHomePage(SecureHomeViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
