using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Generated;
using Plugin.Maui.MVVMExpress.Samples.Generated;
using Plugin.Maui.MVVMExpress.Samples.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.State;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Generated;

public sealed class GeneratedCatalogTests
{
    [Fact]
    public void Notify_AndCommand_Work()
    {
        var vm = new GeneratedCatalogViewModel();
        var names = new List<string>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName ?? "");
        vm.Query = "latte";
        Assert.Equal("Q: latte", vm.Label);
        Assert.Contains(nameof(GeneratedCatalogViewModel.Query), names);
        Assert.Contains(nameof(GeneratedCatalogViewModel.Label), names);
        vm.ClearCommand.Execute(null);
        Assert.Equal("", vm.Query);
    }

    [Fact]
    public async Task Persist_RoundTripsDraft()
    {
        var store = new MemoryStateStore();
        var vm = new GeneratedCatalogViewModel { Draft = "keep" };
        await PersistState.SaveAsync(vm, store);
        var other = new GeneratedCatalogViewModel();
        await PersistState.RestoreAsync(other, store);
        Assert.Equal("keep", other.Draft);
    }

    [Fact]
    public async Task Registration_ResolvesWithoutReflectionScan()
    {
        await using var provider = SampleHarness.CreateProvider();
        Assert.NotNull(provider.GetService(typeof(GeneratedCatalogViewModel)));
        Assert.NotNull(MvvmExpressGeneratedRegistrations.AuthPolicy);
    }

    [Fact]
    public async Task DeepLink_NavigatesMappedRoute()
    {
        var navigator = new FakeNavigator().Map<Plugin.Maui.MVVMExpress.Samples.Crud.ProductListViewModel>("products");
        var map = new DeepLinkRouteMap();
        var result = await map.NavigateAsync(new Uri("app://host/products?q=1"), navigator);
        Assert.True(result.IsSuccess);
        Assert.Equal(typeof(Plugin.Maui.MVVMExpress.Samples.Crud.ProductListViewModel), navigator.Current);
        Assert.Equal("1", navigator.History[0].Query?["q"]?.ToString());
    }
}
