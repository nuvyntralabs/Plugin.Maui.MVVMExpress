using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Pagination;

namespace Plugin.Maui.MVVMExpress.Samples.Playground;

public sealed record PlaygroundItem(string Name);

[RegisterViewModel]
public sealed class PlaygroundListViewModel : PageViewModel
{
    public PlaygroundListViewModel()
    {
        Items = new SnapshotCollection<PlaygroundItem>(_ => Task.FromResult<IReadOnlyList<PlaygroundItem>>(
        [
            new("Alpha"),
            new("Beta"),
            new("Gamma")
        ]));
    }

    public SnapshotCollection<PlaygroundItem> Items { get; }

    /// <inheritdoc />
    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => Items.LoadAsync(cancellationToken: cancellationToken);
}
