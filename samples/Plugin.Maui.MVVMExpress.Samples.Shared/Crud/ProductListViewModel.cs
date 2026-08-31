using Plugin.Maui.MVVMExpress.Collections;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Messaging;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Samples.Crud;

public sealed class ProductListViewModel : PageViewModel
{
    private readonly IProductCatalog _catalog;
    private readonly IMessageHub _hub;

    public ProductListViewModel(IProductCatalog catalog, IMessageHub hub)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(hub);
        _catalog = catalog;
        _hub = hub;
        RefreshCommand = new AsyncModelCommand(ct => Products.LoadAsync(_catalog.ListAsync, ct));
        DeleteCommand = new AsyncModelCommand<int>(DeleteAsync);
        Products.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AsyncState<IReadOnlyList<Product>>.Data) && Products.Data is { } data)
            {
                Items.ReplaceRange(data);
            }
        };
    }

    public AsyncState<IReadOnlyList<Product>> Products { get; } = new();

    public ObservableRangeCollection<Product> Items { get; } = [];

    public AsyncModelCommand RefreshCommand { get; }

    public AsyncModelCommand<int> DeleteCommand { get; }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => RefreshCommand.ExecuteAsync(cancellationToken);

    public override Task OnAppearingAsync(CancellationToken cancellationToken = default)
        => Products.Status is ViewModelStatus.Idle
            ? RefreshCommand.ExecuteAsync(cancellationToken)
            : Task.CompletedTask;

    private async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _catalog.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(result.Error?.Message ?? "Delete failed");
        }

        await RefreshCommand.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        _hub.Publish(new ProductsChanged(Items.Count));
    }
}
