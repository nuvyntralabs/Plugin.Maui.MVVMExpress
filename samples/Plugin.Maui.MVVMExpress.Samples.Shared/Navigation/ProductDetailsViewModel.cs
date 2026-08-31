using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

public sealed record ProductDetailsArgs(int ProductId);

public sealed class ProductDetailsViewModel : PageViewModel, IAcceptNavArgs<ProductDetailsArgs>
{
    private readonly IProductCatalog _catalog;
    private int _productId;

    public ProductDetailsViewModel(IProductCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        _catalog = catalog;
        LoadCommand = new AsyncModelCommand(LoadAsync);
    }

    public AsyncState<Product?> Product { get; } = new();

    public AsyncModelCommand LoadCommand { get; }

    public int ProductId => _productId;

    public void Accept(ProductDetailsArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        _productId = args.ProductId;
    }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => LoadCommand.ExecuteAsync(cancellationToken);

    public override Task OnNavigatedToAsync(CancellationToken cancellationToken = default)
        => InitializeAsync(cancellationToken);

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        await Product.LoadAsync(
            async ct => await _catalog.GetAsync(_productId, ct).ConfigureAwait(false),
            cancellationToken).ConfigureAwait(false);
    }
}
