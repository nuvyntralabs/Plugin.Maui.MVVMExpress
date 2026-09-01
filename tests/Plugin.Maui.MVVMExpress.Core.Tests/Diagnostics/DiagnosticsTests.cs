using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Diagnostics;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Diagnostics;

public sealed class DiagnosticsTests
{
    [Fact]
    public void NullDiagnostics_IsDisabled()
    {
        var sink = new List<string>();
        NullDiagnostics.Instance.Trace("nav", "ignored");
        Assert.False(NullDiagnostics.Instance.IsEnabled);
        var enabled = new CallbackDiagnostics((area, message) => sink.Add($"{area}:{message}"));
        enabled.Trace("nav", "go");
        Assert.Equal("nav:go", Assert.Single(sink));
    }

    [Fact]
    public void AddMvvmExpress_RegistersNullDiagnosticsAndStateStore()
    {
        using var provider = new ServiceCollection().AddMvvmExpress().BuildServiceProvider();
        Assert.Same(NullDiagnostics.Instance, provider.GetRequiredService<IMvvmExpressDiagnostics>());
        Assert.IsType<MemoryStateStore>(provider.GetRequiredService<IStateStore>());
    }
}
