using Plugin.Maui.MVVMExpress.Caching;
using Plugin.Maui.MVVMExpress.Connectivity;
using Plugin.Maui.MVVMExpress.Samples.Offline;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Offline;

public sealed class OfflineCatalogTests
{
    [Fact]
    public async Task CacheFirst_ServesCache_WhenNetworkFails()
    {
        var (inner, _, _, _) = SampleHarness.Core();
        var cache = new MemoryCache();
        var catalog = new CacheFirstCatalog(inner, cache);
        var connectivity = new InMemoryConnectivityProbe();
        var vm = new OfflineCatalogViewModel(catalog, connectivity);
        await vm.InitializeAsync();
        Assert.Equal(5, vm.Items.Count);
        Assert.False(vm.ServedFromCache);

        inner.Offline = true;
        connectivity.IsOnline = false;
        await vm.RefreshCommand.ExecuteAsync();
        Assert.Equal(5, vm.Items.Count);
        Assert.True(vm.ServedFromCache);
        Assert.True(vm.Products.IsSuccess);
    }

    [Fact]
    public async Task NoCache_PropagatesFailure()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        catalog.Offline = true;
        var vm = new OfflineCatalogViewModel(catalog, new InMemoryConnectivityProbe { IsOnline = false });
        await Assert.ThrowsAsync<InvalidOperationException>(() => vm.InitializeAsync());
        Assert.True(vm.Products.HasError);
        Assert.False(vm.ServedFromCache);
    }
}
