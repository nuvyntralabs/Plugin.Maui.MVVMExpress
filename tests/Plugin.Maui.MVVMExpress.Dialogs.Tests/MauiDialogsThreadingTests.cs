using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Outcome;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Dialogs.Tests;

public sealed class MauiDialogsThreadingTests
{
    [Fact]
    public async Task AlertAsync_InvokesMainThread_BeforePageResolve()
    {
        var main = new RecordingMainThread { IsMainThread = false };
        var dialogs = new MauiDialogs(mainThread: main);
        await Assert.ThrowsAsync<InvalidOperationException>(() => dialogs.AlertAsync("t", "m"));
        Assert.True(main.InvokeCount >= 1);
    }

    [Fact]
    public async Task ConfirmAsync_InvokesMainThread()
    {
        var main = new RecordingMainThread { IsMainThread = false };
        var dialogs = new MauiDialogs(mainThread: main);
        await Assert.ThrowsAsync<InvalidOperationException>(() => dialogs.ConfirmAsync("t", "m"));
        Assert.True(main.InvokeCount >= 1);
    }

    [Fact]
    public async Task ErrorAsync_InvokesMainThread()
    {
        var main = new RecordingMainThread { IsMainThread = false };
        var dialogs = new MauiDialogs(mainThread: main);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => dialogs.ErrorAsync(new ErrorInfo("E_X", "failed")));
        Assert.True(main.InvokeCount >= 1);
    }
}
