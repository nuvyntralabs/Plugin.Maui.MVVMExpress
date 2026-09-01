using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Pagination;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Samples.ChatHost;

/// <summary>
/// Load-once inbox. Filter from <see cref="SearchQuery.CommittedText"/> (Entry, not SearchBar).
/// Hub updates go through <see cref="CoalescingDispatcher"/> — they do not call RefreshAsync.
/// </summary>
public sealed class ChatInboxViewModel : PageViewModel
{
    private readonly List<ChatConversation> _all;
    private readonly CoalescingDispatcher _inbox;

    /// <summary>Creates an inbox from a static snapshot.</summary>
    public ChatInboxViewModel(IReadOnlyList<ChatConversation> seed)
    {
        ArgumentNullException.ThrowIfNull(seed);
        _all = [.. seed];
        Search = new SearchQuery(TimeSpan.FromMilliseconds(200));
        Search.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(SearchQuery.CommittedText))
            {
                ApplyFilter();
            }
        };
        Chats = new SnapshotCollection<ChatConversation>(_ =>
            Task.FromResult<IReadOnlyList<ChatConversation>>(Filter(_all, Search.CommittedText)));
        _inbox = new CoalescingDispatcher(ApplyFilter);
        foreach (var row in _all)
        {
            Chats.AddLocal(row);
        }
    }

    /// <summary>Bind an Entry to <see cref="SearchQuery.Text"/>.</summary>
    public SearchQuery Search { get; }

    /// <summary>Load-once list. Do not bind RemainingItemsThreshold.</summary>
    public SnapshotCollection<ChatConversation> Chats { get; }

    /// <inheritdoc />
    public override Task InitializeAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public override Task OnAppearingAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <summary>Local insert after send / inbound — does not reset the collection when unfiltered.</summary>
    public void Receive(ChatConversation row)
    {
        ArgumentNullException.ThrowIfNull(row);
        _all.Insert(0, row);
        if (string.IsNullOrWhiteSpace(Search.CommittedText))
        {
            Chats.Items.Insert(0, row);
            return;
        }

        _inbox.Post();
    }

    private void ApplyFilter()
    {
        var visible = Filter(_all, Search.CommittedText);
        Chats.Items.Clear();
        foreach (var row in visible)
        {
            Chats.AddLocal(row);
        }
    }

    private static IReadOnlyList<ChatConversation> Filter(IEnumerable<ChatConversation> source, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return source.ToArray();
        }

        return source
            .Where(item => item.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }
}
