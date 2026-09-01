using Plugin.Maui.MVVMExpress.Messaging;
using Plugin.Maui.MVVMExpress.Samples.Enterprise;
using Plugin.Maui.MVVMExpress.Samples.Reactive;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Memory;

public sealed class SampleLeakTests
{
    [Fact]
    public void Enterprise_WeakHub_CollectsShell()
    {
        var hub = new MessageHub();
        var weak = SubscribeEnterprise(hub);
        Assert.True(LeakProbe.IsCollected(weak), "Enterprise shell was not collected while the hub lived.");
    }

    [Fact]
    public async Task Search_Dispose_IsCollectable()
    {
        var weak = await CreateSearchAsync();
        Assert.True(LeakProbe.IsCollected(weak), "SearchViewModel was not collected after dispose.");
    }

    [Fact]
    public void FakeMessageHub_WeakSubscriber_IsCollectable()
    {
        var hub = new FakeMessageHub();
        var weak = SubscribeFake(hub);
        Assert.True(LeakProbe.IsCollected(weak));
        hub.Publish(new ProductsChanged(1));
        Assert.Single(hub.Published);
    }

    private static WeakReference SubscribeEnterprise(MessageHub hub)
    {
        var (catalog, _, errors, busy) = SampleHarness.Core();
        var vm = new EnterpriseShellViewModel(
            catalog,
            new InMemoryAuthState(),
            new FakeConnectivity(),
            hub,
            errors,
            busy,
            new FakeMainThread(),
            new FakeNavigator());
        hub.Publish(new ProductsChanged(1));
        Assert.Equal(1, vm.Notices);
        var weak = LeakProbe.Track(vm);
        return weak;
    }

    private static async Task<WeakReference> CreateSearchAsync()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        var vm = new SearchViewModel(catalog, TimeSpan.Zero);
        await vm.SearchCommand.ExecuteAsync();
        vm.First = "Ada";
        vm.Dispose();
        return LeakProbe.Track(vm);
    }

    private static WeakReference SubscribeFake(FakeMessageHub hub)
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        var vm = new Plugin.Maui.MVVMExpress.Samples.Crud.ProductListViewModel(catalog, hub);
        return LeakProbe.Track(vm);
    }
}
