using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Navigation;

public sealed class DeepLinkBridgeTests
{
    [Fact]
    public async Task Navigate_RelativeUri_OpensDetailsWithQuery()
    {
        var navigator = new InMemoryNavigator().Map<ProductDetailsViewModel>("details");
        var bridge = new SampleDeepLinkBridge();
        var result = await bridge.NavigateAsync("details?ProductId=2", navigator);
        Assert.True(result.IsSuccess);
        Assert.Equal(typeof(ProductDetailsViewModel), navigator.Current);
        Assert.Equal("2", navigator.History[0].Query?["ProductId"]?.ToString());
    }

    [Fact]
    public async Task Home_OpenDeepLink_UsesBridge()
    {
        var navigator = new InMemoryNavigator().Map<ProductDetailsViewModel>("details");
        var vm = new HomeViewModel(navigator, deepLinks: new SampleDeepLinkBridge());
        await vm.OpenDeepLinkCommand.ExecuteAsync();
        Assert.Equal(typeof(ProductDetailsViewModel), navigator.Current);
    }
}
