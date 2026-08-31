using Plugin.Maui.MVVMExpress.Caching;
using Plugin.Maui.MVVMExpress.Samples.Models;

namespace Plugin.Maui.MVVMExpress.Samples.Services;

public sealed class CacheFirstCatalog : IProductCatalog
{
    public const string ListKey = "products";

    private readonly IProductCatalog _inner;
    private readonly ICache _cache;

    public CacheFirstCatalog(IProductCatalog inner, ICache cache)
    {
        ArgumentNullException.ThrowIfNull(inner);
        ArgumentNullException.ThrowIfNull(cache);
        _inner = inner;
        _cache = cache;
    }

    public async Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var fresh = await _inner.ListAsync(cancellationToken).ConfigureAwait(false);
            await _cache.SetAsync(ListKey, fresh, cancellationToken).ConfigureAwait(false);
            return fresh;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            var cached = await _cache.GetAsync<IReadOnlyList<Product>>(ListKey, cancellationToken).ConfigureAwait(false);
            if (cached is not null)
            {
                return cached;
            }

            throw;
        }
    }

    public Task<IReadOnlyList<Product>> ListPageAsync(int skip, int take, CancellationToken cancellationToken = default)
        => _inner.ListPageAsync(skip, take, cancellationToken);

    public Task<IReadOnlyList<Product>> SearchAsync(string query, CancellationToken cancellationToken = default)
        => _inner.SearchAsync(query, cancellationToken);

    public Task<Product?> GetAsync(int id, CancellationToken cancellationToken = default)
        => _inner.GetAsync(id, cancellationToken);

    public Task<Op.Outcome<Product>> SaveAsync(Product product, CancellationToken cancellationToken = default)
        => _inner.SaveAsync(product, cancellationToken);

    public Task<Op.Outcome> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => _inner.DeleteAsync(id, cancellationToken);
}
