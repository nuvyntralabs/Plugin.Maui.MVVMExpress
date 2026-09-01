using Plugin.Maui.MVVMExpress.Composition;

namespace Plugin.Maui.MVVMExpress.Samples.ChatHost;

/// <summary>One host, four sections — no <c>GoToAsync</c> and no <c>window.Page</c> swap on tab.</summary>
public sealed class ChatHostViewModel : SectionHostViewModel
{
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
