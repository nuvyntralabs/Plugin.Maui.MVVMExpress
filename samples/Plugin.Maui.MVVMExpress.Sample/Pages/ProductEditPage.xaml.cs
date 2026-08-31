using Plugin.Maui.MVVMExpress.Samples.Crud;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class ProductEditPage : SampleContentPage
{
    public ProductEditPage(ProductEditViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
