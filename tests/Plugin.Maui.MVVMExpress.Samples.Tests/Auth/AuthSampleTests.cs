using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Auth;
using Plugin.Maui.MVVMExpress.Samples.Services;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Auth;

public sealed class AuthSampleTests
{
    [Fact]
    public async Task Login_WrongPassword_DoesNotNavigate()
    {
        var auth = new InMemoryAuthState();
        var navigator = new InMemoryNavigator();
        var guarded = new GuardedNavigator(navigator, auth, typeof(SecureHomeViewModel));
        var vm = new LoginViewModel(auth, guarded) { UserName = "ada", Password = "nope" };
        await vm.SignInCommand.ExecuteAsync();
        Assert.False(auth.IsAuthenticated);
        Assert.Equal("E_AUTH", vm.LastResult?.Error?.Code);
        Assert.Empty(navigator.History);
    }

    [Fact]
    public async Task Login_Success_NavigatesHome()
    {
        var auth = new InMemoryAuthState();
        var navigator = new InMemoryNavigator();
        var guarded = new GuardedNavigator(navigator, auth, typeof(SecureHomeViewModel));
        var vm = new LoginViewModel(auth, guarded) { UserName = "ada", Password = "secret" };
        await vm.SignInCommand.ExecuteAsync();
        Assert.True(auth.IsAuthenticated);
        Assert.True(vm.LastResult?.IsSuccess);
        Assert.Equal(typeof(SecureHomeViewModel), navigator.History[0].ViewModelType);
    }

    [Fact]
    public void Login_CanExecute_RequiresBothFields()
    {
        var vm = new LoginViewModel(new InMemoryAuthState(), new InMemoryNavigator());
        Assert.False(vm.SignInCommand.CanExecute(null));
        vm.UserName = "ada";
        Assert.False(vm.SignInCommand.CanExecute(null));
        vm.Password = "secret";
        Assert.True(vm.SignInCommand.CanExecute(null));
    }

    [Fact]
    public async Task Guard_BlocksSecureHome_WhenAnonymous()
    {
        var auth = new InMemoryAuthState();
        var navigator = new InMemoryNavigator();
        var guarded = new GuardedNavigator(navigator, auth, typeof(SecureHomeViewModel));
        var result = await guarded.NavigateToAsync<SecureHomeViewModel>();
        Assert.False(result.IsSuccess);
        Assert.Equal("E_AUTH", result.Error?.Code);
        Assert.Empty(navigator.History);
    }

    [Fact]
    public async Task SignOut_ClearsAuth_AndGoesBack()
    {
        var auth = new InMemoryAuthState();
        await auth.SignInAsync("ada", "secret");
        var navigator = new InMemoryNavigator();
        var vm = new SecureHomeViewModel(auth, navigator);
        Assert.Equal("ada", vm.UserName);
        await vm.SignOutCommand.ExecuteAsync();
        Assert.False(auth.IsAuthenticated);
        Assert.Equal("back", navigator.History[0].Args);
    }
}
