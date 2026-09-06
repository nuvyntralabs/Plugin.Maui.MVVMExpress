using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Pagination;

namespace MauiApp1;

[RegisterViewModel]
[Route("list")]
public sealed class ItemListViewModel : PageViewModel
{
    public ItemListViewModel(IItemStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        Items = new SnapshotCollection<Item>(store.ListAsync);
    }

    public SnapshotCollection<Item> Items { get; }

    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => Items.LoadAsync(cancellationToken: cancellationToken);
}
