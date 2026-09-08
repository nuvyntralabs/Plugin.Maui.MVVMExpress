using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Samples.Generated;
using Plugin.Maui.MVVMExpress.Samples.Modules;
using Plugin.Maui.MVVMExpress.Samples.Tests.Support;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Modules;

public sealed class CatalogModuleTests
{
    [Fact]
    public void AddModule_RegistersMarkerAndGeneratedCatalog()
    {
        using var provider = SampleHarness.CreateProvider();
        var marker = provider.GetRequiredService<CatalogModuleMarker>();
        Assert.Equal("generated-catalog", marker.Feature);
        Assert.NotNull(provider.GetRequiredService<GeneratedCatalogViewModel>());
    }
}
