using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Samples.Crud;
using Plugin.Maui.MVVMExpress.Samples.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Navigation;

public sealed class ScopedCatalogFlowTests
{
    [Fact]
    public async Task Initialize_OpensList()
    {
        await using var provider = SampleHarness.CreateProvider();
        var flow = provider.GetRequiredService<ScopedCatalogFlowViewModel>();
        await flow.AppearAsync();
        Assert.IsType<ProductListViewModel>(flow.Current);
        Assert.Equal(5, flow.List?.Items.Count);
        Assert.Equal(1, flow.Depth);
        Assert.Equal("Catalog", flow.CurrentTitle);
        Assert.False(flow.OpenListCommand.CanExecute(null));
    }

    [Fact]
    public async Task OpenDetails_ThenBack_RestoresList()
    {
        await using var provider = SampleHarness.CreateProvider();
        var flow = provider.GetRequiredService<ScopedCatalogFlowViewModel>();
        await flow.AppearAsync();
        await flow.OpenDetailsCommand.ExecuteAsync(2);
        Assert.IsType<ProductDetailsViewModel>(flow.Current);
        Assert.Equal(2, flow.Details?.ProductId);
        Assert.Equal("Latte", flow.Details?.Product.Data?.Name);
        Assert.Equal(2, flow.Depth);
        flow.GoBackCommand.Execute(null);
        Assert.IsType<ProductListViewModel>(flow.Current);
        Assert.Equal(1, flow.Depth);
        Assert.True(flow.List is { IsDisposed: false });
    }

    [Fact]
    public async Task PopDetails_CollectsDetailsViewModel()
    {
        var weak = await OpenDetailsAndPopAsync();
        Assert.True(LeakProbe.IsCollected(weak), "Popped details ViewModel was not collected.");
    }

    [Fact]
    public async Task AppearAsync_LoadsListOnce()
    {
        await using var provider = SampleHarness.CreateProvider();
        var flow = provider.GetRequiredService<ScopedCatalogFlowViewModel>();
        await flow.AppearAsync();
        await flow.AppearAsync();
        Assert.Equal(1, flow.Depth);
        Assert.Equal(5, flow.List?.Items.Count);
    }

    private static async Task<WeakReference> OpenDetailsAndPopAsync()
    {
        await using var provider = SampleHarness.CreateProvider();
        var flow = provider.GetRequiredService<ScopedCatalogFlowViewModel>();
        await flow.InitializeAsync();
        await flow.OpenDetailsCommand.ExecuteAsync(1);
        var weak = LeakProbe.Track(flow.Details!);
        flow.GoBackCommand.Execute(null);
        return weak;
    }
}
