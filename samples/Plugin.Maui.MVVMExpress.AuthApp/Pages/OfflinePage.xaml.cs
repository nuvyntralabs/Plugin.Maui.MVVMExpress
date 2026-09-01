using Plugin.Maui.MVVMExpress.Samples.Offline;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class OfflinePage : SampleContentPage
{
    public OfflinePage(OfflineCatalogViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
