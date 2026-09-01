using Plugin.Maui.MVVMExpress.Samples.Navigation;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class ProductDetailsPage : SampleContentPage, IQueryAttributable
{
    public ProductDetailsPage(ProductDetailsViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(ProductDetailsArgs.ProductId), out var raw)
            && int.TryParse(Convert.ToString(raw), out var id)
            && BindingContext is ProductDetailsViewModel viewModel)
        {
            viewModel.Accept(new ProductDetailsArgs(id));
        }
    }
}
