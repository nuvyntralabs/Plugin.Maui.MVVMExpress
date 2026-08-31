using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Core.Tests.Support;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Navigation;

public sealed class InMemoryNavigatorTests
{
    [Fact]
    public async Task NavigateTo_RecordsTypeAndArgs()
    {
        var navigator = new InMemoryNavigator();
        var result = await navigator.NavigateToAsync<ProbeViewModel, int>(9);
        Assert.True(result.IsSuccess);
        Assert.Equal(typeof(ProbeViewModel), navigator.Current);
        Assert.Equal(9, navigator.History[0].Args);
    }

    [Fact]
    public async Task DirtyGuard_BlocksAndLeavesHistoryEmpty()
    {
        var navigator = new InMemoryNavigator(_ => false) { Current = typeof(ProbeViewModel) };
        var result = await navigator.NavigateToAsync<DependentViewModel>();
        Assert.False(result.IsSuccess);
        Assert.Equal("E_GUARD", result.Error?.Code);
        Assert.Empty(navigator.History);
    }

    [Fact]
    public async Task GoBack_RecordsSentinel()
    {
        var navigator = new InMemoryNavigator();
        var result = await navigator.GoBackAsync();
        Assert.True(result.IsSuccess);
        Assert.Equal("back", navigator.History[0].Args);
    }

    [Fact]
    public async Task Guarded_BlocksUntilAuthenticated()
    {
        var inner = new InMemoryNavigator();
        var auth = new MemoryAuth();
        var guarded = new GuardedNavigator(inner, auth, typeof(ProbeViewModel));
        var blocked = await guarded.NavigateToAsync<ProbeViewModel>();
        Assert.Equal("E_AUTH", blocked.Error?.Code);
        auth.IsAuthenticated = true;
        var ok = await guarded.NavigateToAsync<ProbeViewModel>();
        Assert.True(ok.IsSuccess);
        Assert.Equal(typeof(ProbeViewModel), inner.History[0].ViewModelType);
    }

    private sealed class MemoryAuth : IAuthState
    {
        public bool IsAuthenticated { get; set; }

        public string? UserName { get; set; }

        public Task<Plugin.Maui.MVVMExpress.Outcome.Outcome> SignInAsync(
            string userName,
            string password,
            CancellationToken cancellationToken = default)
        {
            IsAuthenticated = true;
            UserName = userName;
            return Task.FromResult(Plugin.Maui.MVVMExpress.Outcome.Outcome.Success());
        }

        public Task SignOutAsync(CancellationToken cancellationToken = default)
        {
            IsAuthenticated = false;
            UserName = null;
            return Task.CompletedTask;
        }
    }
}
