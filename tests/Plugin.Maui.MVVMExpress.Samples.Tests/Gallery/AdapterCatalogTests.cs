using Plugin.Maui.MVVMExpress.Samples.Gallery;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Gallery;

public sealed class AdapterCatalogTests
{
    [Fact]
    public void DocumentsOfficialSiblingPackages()
    {
        var vm = new AdapterCatalogViewModel();
        Assert.Equal("Plugin.Maui.FormValidation", vm.FormValidationPackage);
        Assert.Equal("Plugin.Maui.KeyboardManager", vm.KeyboardPackage);
        Assert.Equal("Plugin.Maui.DeepLinks", vm.DeepLinksPackage);
        Assert.Equal("Plugin.Maui.SecureSession", vm.SecureSessionPackage);
    }
}
