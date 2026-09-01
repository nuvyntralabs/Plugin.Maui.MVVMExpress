using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.AuthApp;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.AuthApp;

public sealed class AuthAppTests
{
    [Fact]
    public async Task Login_ResetAsync_ReplacesRoot()
    {
        var auth = new InMemoryAuthState();
        var dialogs = new FakeDialogs();
        var inner = new InMemoryNavigator()
            .Map<AuthLoginViewModel>("//login")
            .Map<AuthHomeViewModel>("//home");
        var navigator = new GuardedNavigator(inner, auth, typeof(AuthHomeViewModel));
        var vm = new AuthLoginViewModel(auth, navigator, dialogs);
        await vm.SignInCommand.ExecuteAsync();
        Assert.True(auth.IsAuthenticated);
        Assert.Equal(typeof(AuthHomeViewModel), inner.Current);
        Assert.False(inner.CanGoBack);
    }

    [Fact]
    public async Task Register_MustMatch_BlocksSubmit()
    {
        var accounts = new MemoryAccountService();
        var auth = new InMemoryAuthState();
        var dialogs = new FakeDialogs();
        var navigator = new InMemoryNavigator();
        var vm = new AuthRegisterViewModel(accounts, auth, navigator, dialogs)
        {
            Email = "new@mvvmexpress.dev",
            Password = "a",
            Confirm = "b"
        };
        await vm.SubmitCommand.ExecuteAsync();
        Assert.False(auth.IsAuthenticated);
        Assert.Empty(navigator.History);
    }
}
