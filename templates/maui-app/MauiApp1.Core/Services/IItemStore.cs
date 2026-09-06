namespace MauiApp1;

public interface IItemStore
{
    Task<IReadOnlyList<Item>> ListAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(Item item, CancellationToken cancellationToken = default);
}
