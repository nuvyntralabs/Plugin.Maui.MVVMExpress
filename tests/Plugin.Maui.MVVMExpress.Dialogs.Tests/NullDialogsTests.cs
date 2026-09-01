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

    [Fact]
    public async Task NullDialogs_Cancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(() => NullDialogs.Instance.AlertAsync("t", "m", cancellationToken: cts.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => NullDialogs.Instance.ConfirmAsync("t", "m", cancellationToken: cts.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => NullDialogs.Instance.ErrorAsync(new ErrorInfo("E", "m"), cts.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => NullDialogs.Instance.ToastAsync("hi", cancellationToken: cts.Token));
    }

    [Fact]
    public async Task FakeDialogs_RecordsAlertConfirmToast()
    {
        var dialogs = new Plugin.Maui.MVVMExpress.Testing.FakeDialogs { ConfirmResult = false };
        await dialogs.AlertAsync("A", "m");
        Assert.False(await dialogs.ConfirmAsync("C", "m"));
        await dialogs.ToastAsync("ok");
        await dialogs.ErrorAsync(new ErrorInfo("E", "boom"));
        Assert.Equal(["A:m", "confirm:C", "toast:ok", "Error:boom"], dialogs.Alerts);
    }
}
