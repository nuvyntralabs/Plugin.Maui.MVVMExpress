using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Composition;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Memory;

public sealed class NavigationPopGcTests
{
    [Fact]
    public void ScopedNavigator_Pop_CollectsViewModel()
    {
        var weak = PushAndPop();
        Assert.True(LeakProbe.IsCollected(weak), "Popped ViewModel was not collected.");
    }

    [Fact]
    public void ScopedNavigator_Dispose_CollectsEntireStack()
    {
        var weaks = PushTwoAndDisposeHost();
        Assert.All(weaks, weak => Assert.True(LeakProbe.IsCollected(weak)));
    }

    [Fact]
    public void ScopedNavigator_Pop_Empty_Throws()
    {
        using var provider = new ServiceCollection()
            .AddMvvmExpress()
            .AddTransient<PageVm>()
            .BuildServiceProvider();
        using var host = new ScopedNavigator(provider.GetRequiredService<IViewModelScopeFactory>());
        Assert.Throws<InvalidOperationException>(() => host.Pop());
    }

    private static WeakReference PushAndPop()
    {
        using var provider = new ServiceCollection()
            .AddMvvmExpress()
            .AddTransient<PageVm>()
            .BuildServiceProvider();
        using var host = new ScopedNavigator(provider.GetRequiredService<IViewModelScopeFactory>());
        var vm = host.Push<PageVm>();
        var weak = LeakProbe.Track(vm);
        host.Pop();
        Assert.Null(host.Current);
        return weak;
    }

    private static WeakReference[] PushTwoAndDisposeHost()
    {
        using var provider = new ServiceCollection()
            .AddMvvmExpress()
            .AddTransient<PageVm>()
            .BuildServiceProvider();
        var host = new ScopedNavigator(provider.GetRequiredService<IViewModelScopeFactory>());
        var first = host.Push<PageVm>();
        var second = host.Push<PageVm>();
        var weaks = new[] { LeakProbe.Track(first), LeakProbe.Track(second) };
        host.Dispose();
        return weaks;
    }

    private sealed class PageVm : ViewModel;
}
