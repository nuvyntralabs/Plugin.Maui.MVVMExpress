using Plugin.Maui.MVVMExpress.Samples.Basic;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Basic;

public sealed class CounterViewModelTests
{
    [Fact]
    public void Increment_UpdatesCountAndLabel()
    {
        var vm = new CounterViewModel();
        var names = new List<string>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName ?? "");
        vm.IncrementCommand.Execute(null);
        Assert.Equal(1, vm.Count);
        Assert.Equal("Count: 1", vm.Label);
        Assert.Contains(nameof(CounterViewModel.Count), names);
        Assert.Contains(nameof(CounterViewModel.Label), names);
    }

    [Fact]
    public void Decrement_DisabledAtZero()
    {
        var vm = new CounterViewModel();
        Assert.False(vm.DecrementCommand.CanExecute(null));
        vm.DecrementCommand.Execute(null);
        Assert.Equal(0, vm.Count);
        vm.IncrementCommand.Execute(null);
        Assert.True(vm.DecrementCommand.CanExecute(null));
        vm.DecrementCommand.Execute(null);
        Assert.Equal(0, vm.Count);
        Assert.False(vm.ResetCommand.CanExecute(null));
    }

    [Fact]
    public void Reset_ClearsCount()
    {
        var vm = new CounterViewModel();
        vm.IncrementCommand.Execute(null);
        vm.IncrementCommand.Execute(null);
        vm.ResetCommand.Execute(null);
        Assert.Equal(0, vm.Count);
        Assert.False(vm.ResetCommand.CanExecute(null));
    }
}
