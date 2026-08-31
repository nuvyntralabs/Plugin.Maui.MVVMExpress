using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Navigation.Tests;

public sealed class NavigatorTests
{
    [Fact]
    public void NavigationTests_DoNotRequireMauiAtDesignStage()
    {
        Assert.Equal("MVVMExpress", AssemblyMarker.Product);
        Assert.Equal("Plugin.Maui.MVVMExpress.Navigation", NavigationMarker.PackageId);
    }

    [Fact]
    public void MauiShellNavigator_FormatQuery_SerializesPublicProperties()
    {
        var query = MauiShellNavigator.FormatQuery(new SampleArgs(7, "latte"));
        Assert.Contains("Id=7", query);
        Assert.Contains("Name=latte", query);
    }

    [Fact]
    public async Task MauiShellNavigator_WithoutShell_ReturnsE_SHELL()
    {
        var navigator = new MauiShellNavigator().Map<EmptyViewModel>("empty");
        var result = await navigator.NavigateToAsync<EmptyViewModel>();
        Assert.False(result.IsSuccess);
        Assert.Equal("E_SHELL", result.Error?.Code);
    }

    [Fact]
    public async Task MauiShellNavigator_Unmapped_ReturnsE_ROUTE()
    {
        var result = await new MauiShellNavigator().NavigateToAsync<EmptyViewModel>();
        Assert.Equal("E_ROUTE", result.Error?.Code);
    }

    [Fact]
    public async Task InMemoryNavigator_NavigateAndBack()
    {
        var navigator = new InMemoryNavigator();
        Assert.True((await navigator.NavigateToAsync<EmptyViewModel>()).IsSuccess);
        Assert.Equal(typeof(EmptyViewModel), navigator.Current);
        Assert.True((await navigator.GoBackAsync()).IsSuccess);
        Assert.Equal("back", navigator.History[^1].Args);
    }

    private sealed class EmptyViewModel : ViewModel;

    private sealed record SampleArgs(int Id, string Name);
}
