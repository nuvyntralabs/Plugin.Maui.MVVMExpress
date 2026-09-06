namespace MauiApp1;

public sealed class MemoryItemStore : IItemStore
{
    private readonly List<Item> _items =
    [
        new("1", "Alpha"),
        new("2", "Beta"),
        new("3", "Gamma")
    ];

    public Task<IReadOnlyList<Item>> ListAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IReadOnlyList<Item>>(_items.ToArray());
    }

    public Task SaveAsync(Item item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        cancellationToken.ThrowIfCancellationRequested();
        var index = _items.FindIndex(existing => existing.Id == item.Id);
        if (index >= 0)
        {
            _items[index] = item;
        }
        else
        {
            _items.Add(item);
        }

        return Task.CompletedTask;
    }
}
