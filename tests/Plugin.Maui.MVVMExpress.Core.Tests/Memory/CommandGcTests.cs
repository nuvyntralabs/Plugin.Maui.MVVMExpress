using Plugin.Maui.MVVMExpress.Core.Tests.Support;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Memory;

public sealed class CommandGcTests
{
    [Fact]
    public void Command_IsCollectable_WithOwningViewModel()
    {
        var (vm, command) = Create();
        Assert.True(LeakProbe.IsCollected(vm), "ViewModel owning a command was not collected.");
        Assert.True(LeakProbe.IsCollected(command), "ModelCommand was not collected with its ViewModel.");
    }

    [Fact]
    public async Task AsyncCommand_Cancel_StopsWork()
    {
        var vm = new ProbeViewModel();
        var run = vm.WaitCommand.ExecuteAsync();
        Assert.True(vm.WaitCommand.IsRunning);
        vm.WaitCommand.Cancel();
        await run;
        Assert.False(vm.WaitCommand.IsRunning);
        Assert.Equal(CommandExecutionState.Cancelled, vm.WaitCommand.State);
        vm.Dispose();
    }

    private static (WeakReference Vm, WeakReference Command) Create()
    {
        var vm = new ProbeViewModel();
        vm.PingCommand.Execute(null);
        var vmRef = LeakProbe.Track(vm);
        var cmdRef = LeakProbe.Track(vm.PingCommand);
        vm.Dispose();
        return (vmRef, cmdRef);
    }
}
