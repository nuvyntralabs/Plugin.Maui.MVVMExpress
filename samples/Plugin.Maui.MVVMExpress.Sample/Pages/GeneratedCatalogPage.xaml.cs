using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Generated;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(GeneratedCatalogViewModel))]
public partial class GeneratedCatalogPage : SampleContentPage
{
    public GeneratedCatalogPage(GeneratedCatalogViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
