using Plugin.Maui.MVVMExpress.Collections;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Connectivity;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Samples.Offline;

public sealed class OfflineCatalogViewModel : ViewModel
{
    private readonly IProductCatalog _catalog;
    private readonly IConnectivityProbe _connectivity;

    public OfflineCatalogViewModel(IProductCatalog catalog, IConnectivityProbe connectivity)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(connectivity);
        _catalog = catalog;
        _connectivity = connectivity;
        RefreshCommand = new AsyncModelCommand(LoadAsync);
    }

    public AsyncState<IReadOnlyList<Product>> Products { get; } = new();

    public ObservableRangeCollection<Product> Items { get; } = [];

    public bool ServedFromCache { get; private set; }

    public AsyncModelCommand RefreshCommand { get; }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => RefreshCommand.ExecuteAsync(cancellationToken);

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        ServedFromCache = false;
        var online = _connectivity.IsOnline;
        try
        {
            await Products.LoadAsync(_catalog.ListAsync, cancellationToken).ConfigureAwait(false);
            if (Products.Data is { } data)
            {
                Items.ReplaceRange(data);
            }

            ServedFromCache = !online && Items.Count > 0;
            if (ServedFromCache)
            {
                Notify(nameof(ServedFromCache));
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            if (Items.Count > 0)
            {
                ServedFromCache = true;
                Notify(nameof(ServedFromCache));
                return;
            }

            throw;
        }
    }
}
