using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Gallery;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(AdapterCatalogViewModel))]
public partial class AdapterCatalogPage : SampleContentPage
{
    public AdapterCatalogPage(AdapterCatalogViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
