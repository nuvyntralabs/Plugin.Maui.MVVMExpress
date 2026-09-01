using CommunityToolkit.Mvvm.Messaging;
using Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit;

namespace Plugin.Maui.MVVMExpress.Compatibility.Tests;

public sealed class CommunityToolkitMessageHubTests
{
    [Fact]
    public void Publish_Delivers_AndUnsubscribeStops()
    {
        var messenger = new StrongReferenceMessenger();
        var hub = new CommunityToolkitMessageHub(messenger);
        var seen = 0;
        var listener = new object();
        hub.Subscribe<object, string>(listener, (_, _) => seen++);
        hub.Publish("a");
        Assert.Equal(1, seen);
        hub.Unsubscribe(listener);
        hub.Publish("b");
        Assert.Equal(1, seen);
    }

    [Fact]
    public void Publish_ValueType_Delivers()
    {
        var hub = new CommunityToolkitMessageHub(new StrongReferenceMessenger());
        var seen = 0;
        hub.Subscribe<object, int>(new object(), (_, value) => seen = value);
        hub.Publish(7);
        Assert.Equal(7, seen);
    }
}
