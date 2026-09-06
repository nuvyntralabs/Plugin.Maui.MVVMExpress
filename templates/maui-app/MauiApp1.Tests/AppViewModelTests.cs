using MauiApp1;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Testing;
using Xunit;

namespace MauiApp1.Tests;

public sealed class AppViewModelTests
{
    [Fact]
    public async Task MainPage_Increment_UpdatesBindingSource()
    {
        var vm = new MainPageViewModel(new GreetingService(), new InMemoryNavigator(), new FakeDialogs());
        Assert.Equal("Hello, MVVMExpress", vm.Message);

        await vm.IncrementCommand.ExecuteAsync();

        Assert.Equal(1, vm.Count);
        Assert.Equal("Hello, click 1", vm.Message);
    }

    [Fact]
    public async Task MainPage_OpenList_NavigatesToItemList()
    {
        var navigator = new InMemoryNavigator().Map<ItemListViewModel>("list");
        var vm = new MainPageViewModel(new GreetingService(), navigator, new FakeDialogs());

        await vm.OpenListCommand.ExecuteAsync();

        Assert.Equal(typeof(ItemListViewModel), navigator.Current);
    }

    [Fact]
    public async Task SignIn_WithDemoCredentials_ReplacesRootWithMainPage()
    {
        var auth = new DemoAuthState();
        var navigator = new InMemoryNavigator()
            .Map<LoginViewModel>("login")
            .Map<MainPageViewModel>("main");
        var vm = new LoginViewModel(auth, navigator, new FakeDialogs());

        await vm.SignInCommand.ExecuteAsync();

        Assert.True(auth.IsAuthenticated);
        Assert.Equal(typeof(MainPageViewModel), navigator.Current);
    }

    [Fact]
    public async Task SignIn_WithBadPassword_ShowsError_AndStaysOnLogin()
    {
        var auth = new DemoAuthState();
        var navigator = new InMemoryNavigator().Map<LoginViewModel>("login");
        var dialogs = new FakeDialogs();
        var vm = new LoginViewModel(auth, navigator, dialogs)
        {
            Password = "wrong"
        };

        await vm.SignInCommand.ExecuteAsync();

        Assert.False(auth.IsAuthenticated);
        Assert.NotEmpty(dialogs.Alerts);
        Assert.NotEqual(typeof(MainPageViewModel), navigator.Current);
    }

    [Fact]
    public async Task List_InitializeAsync_LoadsSeedItems()
    {
        var vm = new ItemListViewModel(new MemoryItemStore());
        await vm.InitializeAsync();

        Assert.Equal(3, vm.Items.Items.Count);
        Assert.Equal("Alpha", vm.Items.Items[0].Name);
    }

    [Fact]
    public async Task Form_Bind_MarksDirty_AndSaveClears()
    {
        var store = new MemoryItemStore();
        var vm = new ItemEditViewModel(store, new InMemoryNavigator(), new FakeDialogs())
        {
            Name = "Delta"
        };

        Assert.True(vm.IsDirty);
        await vm.SaveCommand.ExecuteAsync();
        Assert.False(vm.IsDirty);

        var items = await store.ListAsync();
        Assert.Contains(items, item => item.Name == "Delta");
    }
}
