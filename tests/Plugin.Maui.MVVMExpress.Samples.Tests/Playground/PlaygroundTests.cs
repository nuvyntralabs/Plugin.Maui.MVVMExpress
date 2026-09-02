using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Playground;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Playground;

public sealed class PlaygroundTests
{
    [Fact]
    public async Task Command_IncrementsCount()
    {
        var vm = new PlaygroundCommandViewModel();
        await vm.IncrementCommand.ExecuteAsync();
        Assert.Equal(1, vm.Count);
    }

    [Fact]
    public async Task Home_NavigateToAsync_OpensDetails()
    {
        var navigator = new InMemoryNavigator()
            .Map<PlaygroundDetailsViewModel>("details");
        var vm = new PlaygroundHomeViewModel(navigator, new FakeDialogs());
        await vm.OpenDetailsCommand.ExecuteAsync();
        Assert.Equal(typeof(PlaygroundDetailsViewModel), navigator.Current);
    }

    [Fact]
    public async Task Dialog_Alert_RecordsResult()
    {
        var dialogs = new FakeDialogs();
        var vm = new PlaygroundDialogViewModel(new InMemoryNavigator(), dialogs);
        await vm.AlertCommand.ExecuteAsync();
        Assert.Equal("Alert dismissed", vm.LastResult);
    }

    [Fact]
    public async Task Form_Bind_MarksDirty_AndSaveClears()
    {
        var vm = new PlaygroundFormViewModel(new InMemoryNavigator(), new FakeDialogs())
        {
            Title = "Changed"
        };
        Assert.True(vm.IsDirty);
        await vm.SaveCommand.ExecuteAsync();
        Assert.False(vm.IsDirty);
    }

    [Fact]
    public async Task Auth_AddAuth_OpensLogin_ThenSecure()
    {
        var auth = new InMemoryAuthState();
        var inner = new InMemoryNavigator()
            .Map<PlaygroundLoginViewModel>("login")
            .Map<PlaygroundSecureViewModel>("secure");
        using var provider = new ServiceCollection()
            .AddMvvmExpress()
            .AddSingleton<IAuthState>(auth)
            .AddSingleton(inner)
            .AddSingleton<INavigationAuthPolicy>(new NavigationAuthPolicy(authRequired: [typeof(PlaygroundSecureViewModel)]))
            .AddAuth<PlaygroundLoginViewModel>()
            .BuildServiceProvider();
        var navigator = provider.GetRequiredService<INavigator>();
        var blocked = await navigator.NavigateToAsync<PlaygroundSecureViewModel>();
        Assert.True(blocked.IsSuccess);
        Assert.Equal(typeof(PlaygroundLoginViewModel), inner.Current);

        var login = new PlaygroundLoginViewModel(auth, navigator, new FakeDialogs());
        await login.SignInCommand.ExecuteAsync();
        await Task.Delay(20);
        Assert.True(auth.IsAuthenticated);
        Assert.Equal(typeof(PlaygroundSecureViewModel), inner.Current);
    }

    [Fact]
    public async Task List_LoadAsync_FillsCollectionViewSource()
    {
        var vm = new PlaygroundListViewModel();
        await vm.InitializeAsync();
        Assert.Equal(3, vm.Items.Items.Count);
        Assert.Equal("Alpha", vm.Items.Items[0].Name);
    }
}
