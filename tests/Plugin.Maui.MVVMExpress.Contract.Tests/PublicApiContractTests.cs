using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Forms;
using Plugin.Maui.MVVMExpress.Generated;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Contract.Tests;

public sealed class PublicApiContractTests
{
    [Fact]
    public void Phase7_PublicTypes_StillExist()
    {
        Assert.Contains(typeof(INavigator).GetMethods(), method => method.Name == nameof(INavigator.NavigateToAsync));
        Assert.Contains(typeof(INavigator).GetMethods(), method => method.Name == nameof(INavigator.ResetAsync));
        Assert.Contains(typeof(IPageNavigator).GetMethods(), method => method.Name == nameof(IPageNavigator.ReplaceRootAsync));
        Assert.NotNull(typeof(MVVMExpressServiceCollectionExtensions).GetMethod(nameof(MVVMExpressServiceCollectionExtensions.AddMvvmExpress)));
        Assert.Contains(typeof(MvvmExpressAuthExtensions).GetMethods(), method => method.Name == nameof(MvvmExpressAuthExtensions.AddAuth));
        Assert.NotNull(typeof(FormViewModel).GetMethod(nameof(FormViewModel.Bind)));
        Assert.Contains(typeof(ObservableModel).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public), method => method.Name == "SetProperty");
        Assert.Contains(typeof(InMemoryNavigator).GetMethods(), method => method.Name == nameof(InMemoryNavigator.Map));
        Assert.NotNull(typeof(GeneratedRegistrationHooks).GetMethod(nameof(GeneratedRegistrationHooks.Apply)));
        Assert.NotNull(typeof(IAcceptNavQuery));
        Assert.NotNull(typeof(RegisterViewModelAttribute));
        Assert.NotNull(typeof(RouteAttribute));
        Assert.NotNull(typeof(RequiresAuthAttribute));
    }

    [Fact]
    public void Phase8And9_AdditiveTypes_Exist()
    {
        Assert.NotNull(typeof(NotifyDependsOnAttribute));
        Assert.NotNull(typeof(IModule));
        Assert.NotNull(typeof(IDeepLinkBridge));
        Assert.NotNull(typeof(IPageNavigator).GetMethod(nameof(IPageNavigator.PushModalAsync)));
        Assert.NotNull(typeof(IPageNavigator).GetMethod(nameof(IPageNavigator.PopModalAsync)));
        Assert.NotNull(typeof(ModuleServiceCollectionExtensions).GetMethod(nameof(ModuleServiceCollectionExtensions.AddModule)));
        Assert.NotNull(typeof(SiblingAdapterExtensions).GetMethod(nameof(SiblingAdapterExtensions.AddDeepLinks), [typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection)]));
        Assert.NotNull(typeof(CommunityToolkitViewModelExtensions).GetMethod(nameof(CommunityToolkitViewModelExtensions.AddCommunityToolkitViewModel)));
        Assert.NotNull(typeof(GeneratedRegistrationHooks).GetMethod(nameof(GeneratedRegistrationHooks.ApplyPageMaps)));
    }
}
