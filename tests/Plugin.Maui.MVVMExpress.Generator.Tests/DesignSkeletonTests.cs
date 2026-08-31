using Plugin.Maui.MVVMExpress.SourceGenerators;

namespace Plugin.Maui.MVVMExpress.Generator.Tests;

public sealed class DesignSkeletonTests
{
    [Fact]
    public void Generators_ExposePackageIdentity()
    {
        Assert.Equal("Plugin.Maui.MVVMExpress.SourceGenerators", GeneratorMarker.PackageId);
    }
}
