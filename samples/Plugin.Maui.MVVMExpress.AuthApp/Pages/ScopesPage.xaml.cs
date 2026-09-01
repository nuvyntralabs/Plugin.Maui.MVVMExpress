using Plugin.Maui.MVVMExpress.Samples.Navigation;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class ScopesPage : SampleContentPage
{
    public ScopesPage(ScopedCatalogFlowViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
