using System.Collections.Specialized;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Pagination;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Pagination;

public sealed class PagedProductViewModelTests
{
    [Fact]
    public async Task LoadMore_AddsPages_UntilExhausted()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        catalog.Seed(Enumerable.Range(1, 5).Select(i => new Product { Id = i, Name = $"P{i}", Price = i }));
        var vm = new PagedProductViewModel(catalog, pageSize: 2);
        var resets = 0;
        vm.Items.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                resets++;
            }
        };

        await vm.InitializeAsync();
        Assert.Equal(2, vm.Items.Count);
        Assert.True(vm.HasMore);
        Assert.Equal(1, resets);

        await vm.LoadMoreCommand.ExecuteAsync();
        Assert.Equal(4, vm.Items.Count);
        await vm.LoadMoreCommand.ExecuteAsync();
        Assert.Equal(5, vm.Items.Count);
        Assert.False(vm.HasMore);
        Assert.False(vm.LoadMoreCommand.CanExecute(null));
        Assert.Equal(3, resets);
    }

    [Fact]
    public async Task Refresh_ReplacesFromStart()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        catalog.Seed(Enumerable.Range(1, 3).Select(i => new Product { Id = i, Name = $"P{i}", Price = i }));
        var vm = new PagedProductViewModel(catalog, pageSize: 2);
        await vm.InitializeAsync();
        await vm.LoadMoreCommand.ExecuteAsync();
        Assert.Equal(3, vm.Items.Count);
        await vm.RefreshCommand.ExecuteAsync();
        Assert.Equal(2, vm.Items.Count);
        Assert.True(vm.HasMore);
    }
}
