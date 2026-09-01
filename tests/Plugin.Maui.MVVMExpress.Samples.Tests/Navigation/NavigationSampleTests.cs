using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples;
using Plugin.Maui.MVVMExpress.Samples.Crud;
using Plugin.Maui.MVVMExpress.Samples.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Navigation;

public sealed class NavigationSampleTests
{
    [Fact]
    public async Task Home_OpenProducts_RecordsTypedNavigation()
    {
        var navigator = new InMemoryNavigator();
        var vm = new HomeViewModel(navigator);
        await vm.OpenProductsCommand.ExecuteAsync();
        Assert.Single(navigator.History);
        Assert.Equal(typeof(ProductListViewModel), navigator.History[0].ViewModelType);
    }

    [Fact]
    public async Task Home_OpenDetails_PassesArgs()
    {
        var navigator = new InMemoryNavigator();
        var vm = new HomeViewModel(navigator);
        await vm.OpenDetailsCommand.ExecuteAsync(7);
        var args = Assert.IsType<ProductDetailsArgs>(navigator.History[0].Args);
        Assert.Equal(7, args.ProductId);
        Assert.Equal(typeof(ProductDetailsViewModel), navigator.History[0].ViewModelType);
    }

    [Fact]
    public async Task Details_Accept_LoadsProduct()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        var vm = new ProductDetailsViewModel(catalog);
        vm.Accept(new ProductDetailsArgs(2));
        await vm.InitializeAsync();
        Assert.Equal(2, vm.ProductId);
        Assert.Equal("Latte", vm.Product.Data?.Name);
        Assert.True(vm.Product.IsSuccess);
    }

    [Fact]
    public async Task Details_Missing_IsEmpty()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        var vm = new ProductDetailsViewModel(catalog);
        vm.Accept(new ProductDetailsArgs(99));
        await vm.InitializeAsync();
        Assert.True(vm.Product.IsEmpty);
    }

    [Fact]
    public async Task Home_OpenDetailsByRoute_PassesQuery()
    {
        var navigator = new InMemoryNavigator().Map<ProductDetailsViewModel>("details");
        var vm = new HomeViewModel(navigator);
        await vm.OpenDetailsByRouteCommand.ExecuteAsync();
        Assert.Equal(typeof(ProductDetailsViewModel), navigator.Current);
        Assert.Equal("2", navigator.History[0].Query?["ProductId"]?.ToString());
    }

    [Fact]
    public async Task Home_ShowToast_RecordsNotifier()
    {
        var dialogs = new FakeDialogs();
        var vm = new HomeViewModel(new InMemoryNavigator(), dialogs);
        await vm.ShowToastCommand.ExecuteAsync();
        Assert.Contains("toast:Opened from Navigation sample", dialogs.Alerts);
    }

    [Fact]
    public async Task Details_AcceptQuery_LoadsProduct()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        var vm = new ProductDetailsViewModel(catalog);
        vm.Accept(new Dictionary<string, object> { ["ProductId"] = "2" });
        await vm.InitializeAsync();
        Assert.Equal(2, vm.ProductId);
        Assert.Equal("Latte", vm.Product.Data?.Name);
    }

    [Fact]
    public async Task Home_FromDi_OpenDetailsByRoute_UsesMappedNavigator()
    {
        using var provider = SampleHarness.CreateProvider();
        var home = provider.GetRequiredService<HomeViewModel>();
        var navigator = provider.GetRequiredService<InMemoryNavigator>();
        var pages = provider.GetRequiredService<IPageNavigator>();
        Assert.NotSame(navigator, pages);
        Assert.NotNull(provider.GetRequiredService<INotifier>());
        await home.OpenDetailsByRouteCommand.ExecuteAsync();
        Assert.Equal(typeof(ProductDetailsViewModel), navigator.Current);
        Assert.Equal("2", navigator.History[0].Query?["ProductId"]?.ToString());
    }

    [Fact]
    public async Task Guard_BlocksWhenDirty()
    {
        var navigator = new InMemoryNavigator(_ => false);
        navigator.Current = typeof(ProductEditViewModel);
        var result = await navigator.NavigateToAsync<ProductListViewModel>();
        Assert.False(result.IsSuccess);
        Assert.Equal("E_GUARD", result.Error?.Code);
        Assert.Empty(navigator.History);
    }
}
