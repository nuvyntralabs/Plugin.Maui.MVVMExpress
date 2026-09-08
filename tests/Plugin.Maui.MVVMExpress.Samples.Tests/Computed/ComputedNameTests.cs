using Plugin.Maui.MVVMExpress.Samples.Computed;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Computed;

public sealed class ComputedNameTests
{
    [Fact]
    public void ChangingFirst_NotifiesFullName()
    {
        var vm = new ComputedNameViewModel();
        var names = new List<string>();
        vm.PropertyChanged += (_, e) => names.Add(e.PropertyName ?? "");
        vm.First = "Grace";
        Assert.Equal("Grace Lovelace", vm.FullName);
        Assert.Contains(nameof(ComputedNameViewModel.FullName), names);
    }
}
