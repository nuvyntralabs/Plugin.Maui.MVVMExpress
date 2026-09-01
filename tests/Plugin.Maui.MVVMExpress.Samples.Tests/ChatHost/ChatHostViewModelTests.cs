using Plugin.Maui.MVVMExpress.Samples.ChatHost;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.ChatHost;

public sealed class ChatHostViewModelTests
{
    [Fact]
    public async Task Host_SwitchesSections_WithoutRefreshingInbox()
    {
        var seed = new[]
        {
            new ChatConversation("1", "Ada", "Hello"),
            new ChatConversation("2", "Grace", "Ping")
        };
        var host = new ChatHostViewModel(seed);
        await host.InitializeAsync();
        Assert.Equal("chats", host.CurrentKey);
        Assert.Equal(2, host.Inbox.Chats.Items.Count);

        await host.SelectAsync("calls");
        Assert.Equal("calls", host.CurrentKey);
        Assert.Equal(2, host.Inbox.Chats.Items.Count);

        host.Inbox.Receive(new ChatConversation("3", "Lin", "New"));
        Assert.Equal(3, host.Inbox.Chats.Items.Count);
    }
}
