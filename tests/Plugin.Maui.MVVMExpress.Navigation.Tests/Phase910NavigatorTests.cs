using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Controls;
using Plugin.Maui.MVVMExpress.Generated;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Navigation.Tests;

public sealed class Phase910NavigatorTests
{
    [Fact]
    public async Task InMemoryNavigator_PushAndPopModal()
    {
        var navigator = new InMemoryNavigator();
        Assert.True((await navigator.NavigateToAsync<EmptyViewModel>()).IsSuccess);
        Assert.True((await navigator.PushModalAsync<ModalViewModel>()).IsSuccess);
        Assert.Single(navigator.ModalStack);
        Assert.True(navigator.History[^1].Modal);
        Assert.True((await navigator.PopModalAsync()).IsSuccess);
        Assert.Empty(navigator.ModalStack);
        Assert.Equal(typeof(EmptyViewModel), navigator.Current);
    }

    [Fact]
    public async Task InMemoryNavigator_PopModal_WhenEmpty_ReturnsE_MODAL()
    {
        var result = await new InMemoryNavigator().PopModalAsync();
        Assert.Equal("E_MODAL", result.Error?.Code);
    }

    [Fact]
    public void AbsoluteRoutes_ResolveWithOrWithoutSlashPrefix()
    {
        var table = new NavigationRouteTable()
            .Map<EmptyViewModel>("//home")
            .Map<ModalViewModel>("tabs/inbox");
        Assert.True(table.TryResolve("//home", out var home));
        Assert.Equal(typeof(EmptyViewModel), home);
        Assert.True(table.TryResolve("home", out home));
        Assert.Equal(typeof(EmptyViewModel), home);
        Assert.True(table.TryResolve("tabs/inbox", out var inbox));
        Assert.Equal(typeof(ModalViewModel), inbox);
    }

    [Fact]
    public async Task TwoWindows_DoNotShareStacks()
    {
        var left = new InMemoryNavigator(window: new WindowContext("left"));
        var right = new InMemoryNavigator(window: new WindowContext("right"));
        var registry = new WindowNavigatorRegistry();
        registry.Register(left.Window, left);
        registry.Register(right.Window, right);
        Assert.True((await left.NavigateToAsync<EmptyViewModel>()).IsSuccess);
        Assert.Null(right.Current);
        Assert.Equal(left, registry.GetNavigator(left.Window));
        Assert.NotEqual(right.Current, left.Current);
    }

    [Fact]
    public void GeneratedPageMaps_ApplyToMauiPageNavigator()
    {
        try
        {
            GeneratedRegistrationHooks.ClearForTests();
            GeneratedRegistrationHooks.Add(new PageModule());
            var navigator = new MauiPageNavigator();
            GeneratedRegistrationHooks.ApplyPageMaps((vm, page, route) => navigator.Map(vm, page, route));
            Assert.True(navigator.TryResolve("empty", out var type));
            Assert.Equal(typeof(EmptyViewModel), type);
        }
        finally
        {
            GeneratedRegistrationHooks.ClearForTests();
        }
    }

    [Fact]
    public void SectionHostView_AndMvvmSearch_Construct()
    {
        var host = new SectionHostView();
        Assert.Equal("", host.CurrentKey);
        var search = new MvvmSearch { Text = "latte" };
        Assert.Equal("latte", search.Text);
    }

    [Fact]
    public void MauiPageNavigator_MapTypes_RegistersRoute()
    {
        var navigator = new MauiPageNavigator().Map(typeof(EmptyViewModel), typeof(DummyPage), "empty");
        Assert.True(navigator.TryResolve("empty", out var type));
        Assert.Equal(typeof(EmptyViewModel), type);
    }

    private sealed class EmptyViewModel : ViewModel;

    private sealed class ModalViewModel : ViewModel;

    private sealed class DummyPage : ContentPage;

    private sealed class PageModule : IGeneratedMvvmExpressModule
    {
        public void AddViewModels(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
        }

        public void ApplyRoutes(Action<Type, string> map) => map(typeof(EmptyViewModel), "empty");

        public void ApplyPageMaps(Action<Type, Type, string?> map)
            => map(typeof(EmptyViewModel), typeof(DummyPage), "empty");

        public Plugin.Maui.MVVMExpress.Auth.INavigationAuthPolicy AuthPolicy { get; } =
            new Plugin.Maui.MVVMExpress.Auth.NavigationAuthPolicy();
    }
}
