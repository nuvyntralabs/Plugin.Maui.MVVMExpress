using Plugin.Maui.MVVMExpress.Samples.Auth;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class LoginPage : SampleContentPage
{
    public LoginPage(LoginViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
