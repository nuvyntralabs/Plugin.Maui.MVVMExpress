using Plugin.Maui.MVVMExpress.Samples.Basic;
using Plugin.Maui.MVVMExpress.Samples.Crud;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Memory;

public sealed class SampleViewModelGcTests
{
    [Fact]
    public void Counter_IsCollectable()
    {
        var weak = CreateCounter();
        Assert.True(LeakProbe.IsCollected(weak));
    }

    [Fact]
    public async Task ProductList_IsCollectable_AfterDispose()
    {
        var weak = await CreateListAsync();
        Assert.True(LeakProbe.IsCollected(weak));
    }

    private static WeakReference CreateCounter()
    {
        var vm = new CounterViewModel();
        vm.IncrementCommand.Execute(null);
        vm.Dispose();
        return LeakProbe.Track(vm);
    }

    private static async Task<WeakReference> CreateListAsync()
    {
        var (catalog, hub, _, _) = SampleHarness.Core();
        var vm = new ProductListViewModel(catalog, hub);
        await vm.InitializeAsync();
        vm.Dispose();
        return LeakProbe.Track(vm);
    }
}
