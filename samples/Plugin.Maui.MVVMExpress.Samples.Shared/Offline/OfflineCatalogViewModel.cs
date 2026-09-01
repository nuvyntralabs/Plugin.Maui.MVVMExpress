using Plugin.Maui.MVVMExpress.Caching;
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
    private readonly ICachedFetcher _fetcher;

    public OfflineCatalogViewModel(
        IProductCatalog catalog,
        IConnectivityProbe connectivity,
        ICachedFetcher? fetcher = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(connectivity);
        _catalog = catalog;
        _connectivity = connectivity;
        _fetcher = fetcher ?? new CachedFetcher(new MemoryCache(), connectivity);
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
        var policy = _connectivity.IsOnline ? FetchPolicy.NetworkFirst : FetchPolicy.CacheFirst;
        var fromCache = false;
        try
        {
            await Products.LoadAsync(
                async ct =>
                {
                    var fetch = await _fetcher.FetchAsync(
                        CacheFirstCatalog.ListKey,
                        _catalog.ListAsync,
                        policy,
                        ct).ConfigureAwait(false);
                    fromCache = fetch.FromCache;
                    return fetch.Value ?? (IReadOnlyList<Product>)[];
                },
                cancellationToken).ConfigureAwait(false);
            if (Products.Data is { } data)
            {
                Items.ReplaceRange(data);
            }

            ServedFromCache = fromCache;
            Notify(nameof(ServedFromCache));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception) when (Items.Count > 0)
        {
            ServedFromCache = true;
            Notify(nameof(ServedFromCache));
        }
    }
}
