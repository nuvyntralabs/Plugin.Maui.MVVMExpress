using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Caching;
using Plugin.Maui.MVVMExpress.Connectivity;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Messaging;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Hosting;

public sealed class AddMvvmExpressTests
{
    [Fact]
    public void AddMvvmExpress_RegistersCoreSingletons()
    {
        using var provider = new ServiceCollection().AddMvvmExpress().BuildServiceProvider();
        Assert.IsType<MessageHub>(provider.GetRequiredService<IMessageHub>());
        Assert.IsType<MemoryCache>(provider.GetRequiredService<ICache>());
        Assert.IsType<InMemoryConnectivityProbe>(provider.GetRequiredService<IConnectivityProbe>());
        var navigator = Assert.IsType<InMemoryNavigator>(provider.GetRequiredService<INavigator>());
        Assert.Same(navigator, provider.GetRequiredService<IPageNavigator>());
        Assert.Same(ImmediateMainThread.Instance, provider.GetRequiredService<IMainThread>());
        Assert.IsType<NullDialogs>(provider.GetRequiredService<IDialogs>());
        Assert.Same(NullDialogs.Instance, provider.GetRequiredService<INotifier>());
        Assert.Equal("default", provider.GetRequiredService<IWindowContext>().WindowId);
        Assert.IsType<WindowNavigatorRegistry>(provider.GetRequiredService<IWindowNavigatorRegistry>());
    }
}
