using Plugin.Maui.MVVMExpress.Samples.Enterprise;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class EnterprisePage : SampleContentPage
{
    public EnterprisePage(EnterpriseShellViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
