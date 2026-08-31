using Plugin.Maui.MVVMExpress.Samples.Models;

namespace Plugin.Maui.MVVMExpress.Samples.Services;

public interface IProductCatalog
{
    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListPageAsync(int skip, int take, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> SearchAsync(string query, CancellationToken cancellationToken = default);

    Task<Product?> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Op.Outcome<Product>> SaveAsync(Product product, CancellationToken cancellationToken = default);

    Task<Op.Outcome> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
