using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Samples.Operations;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Operations;

public sealed class PipelineTests
{
    [Fact]
    public async Task Run_RecordsSuccessOutcome()
    {
        using var provider = SampleHarness.CreateProvider();
        var vm = provider.GetRequiredService<PipelineViewModel>();
        await vm.RunCommand.ExecuteAsync();
        Assert.Equal(1, vm.Runs);
        Assert.Equal("ok", vm.Last);
    }
}
