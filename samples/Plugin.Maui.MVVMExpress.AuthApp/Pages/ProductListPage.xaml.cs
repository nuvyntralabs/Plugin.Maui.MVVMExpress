using Plugin.Maui.MVVMExpress.Samples.Crud;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class ProductListPage : SampleContentPage
{
    public ProductListPage(ProductListViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
