using Plugin.Maui.MVVMExpress.Busy;
using Plugin.Maui.MVVMExpress.Connectivity;
using Plugin.Maui.MVVMExpress.Messaging;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Auth;
using Plugin.Maui.MVVMExpress.Samples.Enterprise;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;
using Plugin.Maui.MVVMExpress.State;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Enterprise;

public sealed class EnterpriseShellTests
{
    [Fact]
    public async Task Refresh_LoadsWhenOnline()
    {
        var vm = Create(out _, out _, out var errors, out _);
        await vm.InitializeAsync();
        Assert.True(vm.Products.IsSuccess);
        Assert.Equal(5, vm.Products.Data?.Count);
        Assert.Equal(ViewModelStatus.Success, vm.Status);
        Assert.Empty(errors.Errors);
    }

    [Fact]
    public async Task Refresh_Offline_SetsStatusAndSink()
    {
        var vm = Create(out _, out var connectivity, out var errors, out _);
        connectivity.IsOnline = false;
        await vm.InitializeAsync();
        Assert.Equal(ViewModelStatus.Offline, vm.Status);
        Assert.Equal("E_OFFLINE", errors.Errors[0].Code);
    }

    [Fact]
    public async Task Hub_UpdatesNotices()
    {
        var vm = Create(out var hub, out _, out _, out _);
        hub.Publish(new ProductsChanged(3));
        Assert.Equal(3, vm.Notices);
    }

    [Fact]
    public async Task OpenSecure_BlockedUntilSignedIn()
    {
        var (catalog, hub, errors, busy) = SampleHarness.Core();
        var auth = new InMemoryAuthState();
        var navigator = new InMemoryNavigator();
        var guarded = new GuardedNavigator(navigator, auth, typeof(SecureHomeViewModel));
        var vm = new EnterpriseShellViewModel(
            catalog,
            auth,
            new InMemoryConnectivityProbe(),
            hub,
            errors,
            busy,
            ImmediateMainThread.Instance,
            guarded);
        await vm.OpenSecureCommand.ExecuteAsync();
        Assert.Empty(navigator.History);
        await auth.SignInAsync("ada", "secret");
        await vm.OpenSecureCommand.ExecuteAsync();
        Assert.Equal(typeof(SecureHomeViewModel), navigator.History[0].ViewModelType);
    }

    [Fact]
    public async Task CompositionRoot_ResolvesAllViewModels()
    {
        await using var provider = SampleHarness.CreateProvider();
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Samples.Basic.CounterViewModel)));
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Samples.Crud.ProductListViewModel)));
        Assert.NotNull(provider.GetService(typeof(EnterpriseShellViewModel)));
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Samples.Navigation.ScopedCatalogFlowViewModel)));
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Samples.Generated.GeneratedCatalogViewModel)));
        Assert.NotNull(provider.GetService(typeof(INavigator)));
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Caching.ICache)));
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Validation.IValidator)));
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Flags.IFeatureSwitch)));
        Assert.NotNull(provider.GetService(typeof(Plugin.Maui.MVVMExpress.Operations.IOperationExecutor)));
    }

    [Fact]
    public async Task ChildStatus_IsAttachedAndDisposed()
    {
        var vm = Create(out _, out _, out _, out _);
        Assert.Single(vm.Children);
        Assert.Same(vm.CatalogStatus, vm.Children[0]);
        await vm.InitializeAsync();
        Assert.True(vm.CatalogStatus.IsOnline);
        vm.Dispose();
        Assert.True(vm.CatalogStatus.IsDisposed);
    }

    private static EnterpriseShellViewModel Create(
        out MessageHub hub,
        out InMemoryConnectivityProbe connectivity,
        out RecordingErrorSink errors,
        out InMemoryNavigator navigator)
    {
        var (catalog, createdHub, createdErrors, busy) = SampleHarness.Core();
        hub = createdHub;
        connectivity = new InMemoryConnectivityProbe();
        errors = createdErrors;
        navigator = new InMemoryNavigator();
        var auth = new InMemoryAuthState();
        var guarded = new GuardedNavigator(navigator, auth, typeof(SecureHomeViewModel));
        return new EnterpriseShellViewModel(
            catalog,
            auth,
            connectivity,
            hub,
            errors,
            busy,
            ImmediateMainThread.Instance,
            guarded);
    }
}
