using Plugin.Maui.MVVMExpress.Composition;
using Plugin.Maui.MVVMExpress.Hosting;

namespace Plugin.Maui.MVVMExpress.Samples.ChatHost;

/// <summary>One host, four sections — no <c>GoToAsync</c> and no <c>window.Page</c> swap on tab.</summary>
[Route("chats")]
public sealed class ChatHostViewModel : SectionHostViewModel
{
    /// <summary>Demo inbox used by the Sample flyout and tests.</summary>
    public static IReadOnlyList<ChatConversation> DemoSeed { get; } =
    [
        new("1", "Ada", "Hello"),
        new("2", "Grace", "Ping")
    ];

    /// <summary>Creates a host with chats / updates / communities / calls sections.</summary>
    public ChatHostViewModel(IReadOnlyList<ChatConversation> seed)
    {
        Inbox = Add("chats", new ChatInboxViewModel(seed));
        Add("updates", new ChatInboxViewModel([]));
        Add("communities", new ChatInboxViewModel([]));
        Add("calls", new ChatInboxViewModel([]));
    }

    /// <summary>Visible chats section.</summary>
    public ChatInboxViewModel Inbox { get; }
}
