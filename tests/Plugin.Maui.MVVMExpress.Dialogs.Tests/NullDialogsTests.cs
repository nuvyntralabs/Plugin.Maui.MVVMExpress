using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Outcome;

namespace Plugin.Maui.MVVMExpress.Dialogs.Tests;

public sealed class NullDialogsTests
{
    [Fact]
    public void DialogTests_DoNotRequireMauiAtDesignStage()
    {
        Assert.Equal("MVVMExpress", AssemblyMarker.Product);
    }

    [Fact]
    public async Task NullDialogs_AlertAndConfirm()
    {
        await NullDialogs.Instance.AlertAsync("t", "m");
        Assert.True(await NullDialogs.Instance.ConfirmAsync("t", "m"));
        await NullDialogs.Instance.ErrorAsync(new ErrorInfo("E", "msg"));
        await NullDialogs.Instance.ToastAsync("hi");
    }
}
