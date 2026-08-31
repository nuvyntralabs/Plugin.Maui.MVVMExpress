using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Integration.Tests;

public sealed class DesignSkeletonTests
{
    [Fact]
    public void TestingPackage_IsAvailableForFutureHostTests()
    {
        Assert.Equal("Plugin.Maui.MVVMExpress.Testing", TestingMarker.PackageId);
    }
}
