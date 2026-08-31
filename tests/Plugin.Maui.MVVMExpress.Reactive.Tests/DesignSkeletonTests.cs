using Plugin.Maui.MVVMExpress.Reactive;

namespace Plugin.Maui.MVVMExpress.Reactive.Tests;

public sealed class DesignSkeletonTests
{
    [Fact]
    public void Reactive_ExposesPackageIdentity()
    {
        Assert.Equal("Plugin.Maui.MVVMExpress.Reactive", ReactiveMarker.PackageId);
    }
}
