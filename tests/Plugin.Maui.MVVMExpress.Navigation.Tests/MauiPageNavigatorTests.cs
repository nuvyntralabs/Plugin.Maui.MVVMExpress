using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Navigation.Tests;

public sealed class MauiPageNavigatorTests
{
    [Fact]
    public async Task Unmapped_ReturnsE_ROUTE()
    {
        var navigator = new MauiPageNavigator(new WindowContext("page"));
        var result = await navigator.NavigateToAsync<EmptyViewModel>();
        Assert.Equal("E_ROUTE", result.Error?.Code);
        Assert.Empty(navigator.Stack);
    }

    [Fact]
    public async Task MappedWithoutHost_ReturnsE_PAGE()
    {
        var navigator = new MauiPageNavigator(new WindowContext("page"))
            .Map<EmptyViewModel, DummyPage>("empty");
        Assert.True(navigator.TryResolve("empty?x=1", out var type));
        Assert.Equal(typeof(EmptyViewModel), type);
        var result = await navigator.NavigateToAsync("empty", new Dictionary<string, object> { ["x"] = 1 });
        Assert.Equal("E_PAGE", result.Error?.Code);
        Assert.Empty(navigator.Stack);
        Assert.Equal("page", navigator.Window.WindowId);
    }

    [Fact]
    public async Task StackApisWithoutHost_ReturnE_PAGE()
    {
        var navigator = new MauiPageNavigator().Map<EmptyViewModel, DummyPage>("empty");
        Assert.Equal("E_PAGE", (await navigator.GoBackAsync()).Error?.Code);
        Assert.Equal("E_PAGE", (await navigator.PopToRootAsync()).Error?.Code);
        Assert.Equal("E_PAGE", (await navigator.ReplaceAsync<EmptyViewModel>()).Error?.Code);
        Assert.Equal("E_PAGE", (await navigator.ResetAsync<EmptyViewModel>()).Error?.Code);
    }

    private sealed class EmptyViewModel : ViewModel;

    private sealed class DummyPage : ContentPage;
}
