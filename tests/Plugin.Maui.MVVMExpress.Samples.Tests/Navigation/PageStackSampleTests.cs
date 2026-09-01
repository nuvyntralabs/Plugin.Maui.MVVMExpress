using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Navigation;

public sealed class PageStackSampleTests
{
    [Fact]
    public async Task Push_Pop_PopToRoot_Replace_Reset()
    {
        var navigator = CreateStack();
        await navigator.NavigateToAsync<PageStackViewModel>();
        var dialogs = new FakeDialogs();
        var vm = new PageStackViewModel(navigator, dialogs);

        await vm.PushCommand.ExecuteAsync();
        Assert.Equal(typeof(PageStackItemViewModel), navigator.Current);
        Assert.Equal("Item 2", navigator.History[^1].Query?["Title"]?.ToString());
        Assert.True(navigator.CanGoBack);

        await vm.OnNavigatedToAsync();
        Assert.True(vm.CanGoBack);
        Assert.Equal(2, vm.StackCount);

        await vm.PopCommand.ExecuteAsync();
        Assert.Equal(typeof(PageStackViewModel), navigator.Current);

        await vm.PushCommand.ExecuteAsync();
        await vm.PopToRootCommand.ExecuteAsync();
        Assert.Equal([typeof(PageStackViewModel)], navigator.Stack);

        await vm.ReplaceCommand.ExecuteAsync();
        Assert.Equal(typeof(PageStackItemViewModel), navigator.Current);
        Assert.Equal("Replaced", navigator.History[^1].Query?["Title"]?.ToString());

        await vm.ResetCommand.ExecuteAsync();
        Assert.Equal([typeof(PageStackViewModel)], navigator.Stack);
        Assert.Equal("page-stack", vm.WindowId);
    }

    [Fact]
    public async Task Toast_RecordsWindowAndStack()
    {
        var navigator = CreateStack();
        await navigator.NavigateToAsync<PageStackViewModel>();
        var dialogs = new FakeDialogs();
        var vm = new PageStackViewModel(navigator, dialogs);
        await vm.ToastCommand.ExecuteAsync();
        Assert.Contains(dialogs.Alerts, text => text.StartsWith("toast:Window page-stack", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Item_AcceptQuery_AndPushDeeper()
    {
        var navigator = CreateStack();
        await navigator.NavigateToAsync<PageStackViewModel>();
        var item = new PageStackItemViewModel(navigator);
        item.Accept(new Dictionary<string, object> { ["Title"] = "Latte", ["Depth"] = "3" });
        Assert.Equal("Latte", item.Title);
        Assert.Equal(3, item.Depth);
        await item.PushDeeperCommand.ExecuteAsync();
        Assert.Equal(typeof(PageStackItemViewModel), navigator.Current);
        Assert.Equal("4", navigator.History[^1].Query?["Depth"]?.ToString());
        await item.GoBackCommand.ExecuteAsync();
        Assert.Equal(typeof(PageStackViewModel), navigator.Current);
    }

    [Fact]
    public void Di_ResolvesPageStack()
    {
        using var provider = SampleHarness.CreateProvider();
        var pages = provider.GetRequiredService<IPageNavigator>();
        Assert.Equal("page-stack", pages.Window.WindowId);
        Assert.NotNull(provider.GetRequiredService<PageStackViewModel>());
        Assert.NotNull(provider.GetRequiredService<PageStackItemViewModel>());
    }

    private static InMemoryNavigator CreateStack()
        => new InMemoryNavigator(window: new WindowContext("page-stack"))
            .Map<PageStackViewModel>("stack")
            .Map<PageStackItemViewModel>("stack-item");
}
