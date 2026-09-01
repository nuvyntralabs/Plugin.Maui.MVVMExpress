using Plugin.Maui.MVVMExpress.Samples.Models;

namespace Plugin.Maui.MVVMExpress.Samples.Services;

public sealed class InMemoryProductCatalog : IProductCatalog
{
    private readonly object _gate = new();
    private readonly List<Product> _items;
    private int _nextId;

    public InMemoryProductCatalog()
    {
        _items =
        [
            new Product { Id = 1, Name = "Espresso", Price = 2.50m },
            new Product { Id = 2, Name = "Latte", Price = 4.00m },
            new Product { Id = 3, Name = "Mocha", Price = 4.50m },
            new Product { Id = 4, Name = "Tea", Price = 2.00m },
            new Product { Id = 5, Name = "Cocoa", Price = 3.25m }
        ];
        _nextId = 6;
    }

    public TimeSpan Delay { get; set; }

    public bool FailNext { get; set; }

    public bool Offline { get; set; }

    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _items.Count;
            }
        }
    }

    public Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default)
        => RunAsync(() =>
        {
            lock (_gate)
            {
                return (IReadOnlyList<Product>)_items.ToArray();
            }
        }, cancellationToken);

    public Task<IReadOnlyList<Product>> ListPageAsync(int skip, int take, CancellationToken cancellationToken = default)
        => RunAsync(() =>
        {
            ArgumentOutOfRangeException.ThrowIfNegative(skip);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(take);
            lock (_gate)
            {
                return (IReadOnlyList<Product>)_items.Skip(skip).Take(take).ToArray();
            }
        }, cancellationToken);

    public Task<IReadOnlyList<Product>> SearchAsync(string query, CancellationToken cancellationToken = default)
        => RunAsync(() =>
        {
            query ??= "";
            lock (_gate)
            {
                return (IReadOnlyList<Product>)_items
                    .Where(item => item.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }
        }, cancellationToken);

    public Task<Product?> GetAsync(int id, CancellationToken cancellationToken = default)
        => RunAsync(() =>
        {
            lock (_gate)
            {
                return _items.FirstOrDefault(item => item.Id == id);
            }
        }, cancellationToken);

    public Task<Op.Outcome<Product>> SaveAsync(Product product, CancellationToken cancellationToken = default)
        => RunAsync(() =>
        {
            ArgumentNullException.ThrowIfNull(product);
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return Op.Outcome<Product>.Failure("E_VAL", "Name is required");
            }

            lock (_gate)
            {
                if (product.Id == 0)
                {
                    var created = new Product { Id = _nextId++, Name = product.Name, Price = product.Price };
                    _items.Add(created);
                    return Op.Outcome<Product>.Success(created);
                }

                var index = _items.FindIndex(item => item.Id == product.Id);
                if (index < 0)
                {
                    return Op.Outcome<Product>.Failure("E_NF", "Product not found");
                }

                _items[index] = product;
                return Op.Outcome<Product>.Success(product);
            }
        }, cancellationToken);

    public Task<Op.Outcome> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => RunAsync(() =>
        {
            lock (_gate)
            {
                var removed = _items.RemoveAll(item => item.Id == id) > 0;
                return removed
                    ? Op.Outcome.Success()
                    : Op.Outcome.Failure("E_NF", "Product not found");
            }
        }, cancellationToken);

    public void Seed(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        lock (_gate)
        {
            _items.Clear();
            _items.AddRange(products);
            _nextId = _items.Count == 0 ? 1 : _items.Max(item => item.Id) + 1;
        }
    }

    public void SeedScale(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        Seed(Enumerable.Range(1, count).Select(i => new Product { Id = i, Name = $"P{i}", Price = i }));
    }

    private async Task<T> RunAsync<T>(Func<T> work, CancellationToken cancellationToken)
    {
        if (Delay > TimeSpan.Zero)
        {
            await Task.Delay(Delay, cancellationToken).ConfigureAwait(false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (Offline)
        {
            throw new InvalidOperationException("Network unavailable");
        }

        if (FailNext)
        {
            FailNext = false;
            throw new InvalidOperationException("Catalog failed");
        }

        return work();
    }
}
