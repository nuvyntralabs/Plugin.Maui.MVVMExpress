using Plugin.Maui.MVVMExpress.Samples.EscapeHatch;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.EscapeHatch;

public sealed class ManualCounterTests
{
    [Fact]
    public void SetProperty_StillWorks()
    {
        var vm = new ManualCounterViewModel();
        vm.IncrementCommand.Execute(null);
        Assert.Equal(1, vm.Count);
    }
}
