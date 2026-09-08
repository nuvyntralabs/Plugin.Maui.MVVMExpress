using Plugin.Maui.MVVMExpress.Samples.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Windows;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Windows;

public sealed class TwoWindowDemoTests
{
    [Fact]
    public async Task NavigateA_DoesNotChangeWindowB()
    {
        var vm = new TwoWindowDemoViewModel();
        await vm.NavigateACommand.ExecuteAsync();
        Assert.Equal(typeof(HomeViewModel), vm.WindowA.Current);
        Assert.Null(vm.WindowB.Current);

        await vm.NavigateBCommand.ExecuteAsync();
        Assert.Equal(typeof(HomeViewModel), vm.WindowB.Current);
        Assert.Equal(typeof(HomeViewModel), vm.WindowA.Current);
    }
}
