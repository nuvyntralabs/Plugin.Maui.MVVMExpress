using Plugin.Maui.MVVMExpress.Samples.Reactive;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Reactive;

public sealed class SearchViewModelTests
{
    [Fact]
    public async Task Search_FiltersByName()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        var vm = new SearchViewModel(catalog, TimeSpan.Zero);
        await vm.SearchCommand.ExecuteAsync();
        Assert.Equal(5, vm.Items.Count);
        vm.Query = "lat";
        await Task.Delay(20);
        Assert.Single(vm.Items);
        Assert.Equal("Latte", vm.Items[0].Name);
    }

    [Fact]
    public async Task Debounce_CancelsPrevious()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        catalog.Delay = TimeSpan.FromMilliseconds(40);
        var vm = new SearchViewModel(catalog, TimeSpan.FromMilliseconds(15));
        vm.Query = "e";
        vm.Query = "latte";
        await Task.Delay(120);
        Assert.True(vm.SearchStarts >= 1);
        Assert.Single(vm.Items);
        Assert.Equal("Latte", vm.Items[0].Name);
    }

    [Fact]
    public void FullName_DependsOnFirstAndLast()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        var vm = new SearchViewModel(catalog, TimeSpan.Zero);
        var names = new List<string>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName ?? "");
        vm.First = "Ada";
        vm.Last = "Lovelace";
        Assert.Equal("Ada Lovelace", vm.FullName);
        Assert.Contains(nameof(SearchViewModel.FullName), names);
    }

    [Fact]
    public async Task Dispose_CancelsDebounce()
    {
        var (catalog, _, _, _) = SampleHarness.Core();
        catalog.Delay = TimeSpan.FromSeconds(5);
        var vm = new SearchViewModel(catalog, TimeSpan.FromMilliseconds(50));
        vm.Query = "e";
        vm.Dispose();
        await Task.Delay(80);
        Assert.True(vm.ViewModelCancellationToken.IsCancellationRequested);
    }
}
