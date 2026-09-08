using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Compatibility.Tests;

public sealed class CommunityToolkitViewModelInteropTests
{
    [Fact]
    public async Task ObservableObject_CanInjectNavigatorAndDialogs()
    {
        var services = new ServiceCollection().AddCommunityToolkitViewModel<ToolkitInboxViewModel>();
        var navigator = new FakeNavigator().Map<HomeViewModel>("home");
        var dialogs = new FakeDialogs();
        services.AddSingleton<INavigator>(navigator);
        services.AddSingleton<Plugin.Maui.MVVMExpress.Dialogs.IDialogs>(dialogs);
        using var provider = services.BuildServiceProvider();
        var vm = provider.GetRequiredService<ToolkitInboxViewModel>();
        Assert.True((await vm.OpenAsync()).IsSuccess);
        Assert.Equal(typeof(HomeViewModel), navigator.Current);
        await vm.WarnAsync();
        Assert.Contains(vm.Dialogs.Alerts, item => item.StartsWith("Hi:", StringComparison.Ordinal));
    }

    private sealed class HomeViewModel : Plugin.Maui.MVVMExpress.ComponentModel.ViewModel;

    private sealed class ToolkitInboxViewModel : ObservableObject
    {
        public ToolkitInboxViewModel(INavigator navigator, Plugin.Maui.MVVMExpress.Dialogs.IDialogs dialogs)
        {
            Navigator = navigator;
            Dialogs = (FakeDialogs)dialogs;
        }

        public INavigator Navigator { get; }
        public FakeDialogs Dialogs { get; }

        public Task<Plugin.Maui.MVVMExpress.Outcome.Outcome> OpenAsync()
            => Navigator.NavigateToAsync<HomeViewModel>();

        public Task WarnAsync() => Dialogs.AlertAsync("Hi", "Body");
    }
}
