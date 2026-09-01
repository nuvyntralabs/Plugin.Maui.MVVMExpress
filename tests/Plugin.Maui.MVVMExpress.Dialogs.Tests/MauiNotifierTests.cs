using Plugin.Maui.MVVMExpress.Dialogs;

namespace Plugin.Maui.MVVMExpress.Dialogs.Tests;

public sealed class MauiNotifierTests
{
    [Fact]
    public async Task ToastAsync_UsesPresenter()
    {
        var presenter = new RecordingPresenter();
        var notifier = new MauiNotifier(presenter);
        await notifier.ToastAsync("saved", TimeSpan.FromMilliseconds(10));
        Assert.Equal(["saved"], presenter.Messages);
        Assert.Equal(TimeSpan.FromMilliseconds(10), presenter.LastDuration);
    }

    [Fact]
    public async Task ToastAsync_DefaultDuration_IsTwoSeconds()
    {
        var presenter = new RecordingPresenter();
        await new MauiNotifier(presenter).ToastAsync("hi");
        Assert.Equal(TimeSpan.FromSeconds(2), presenter.LastDuration);
    }

    [Fact]
    public async Task ToastAsync_Cancelled_Throws()
    {
        var notifier = new MauiNotifier(new RecordingPresenter());
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(() => notifier.ToastAsync("x", cancellationToken: cts.Token));
    }

    [Fact]
    public async Task ToastAsync_EmptyMessage_Throws()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => new MauiNotifier(new RecordingPresenter()).ToastAsync(" "));
    }

    private sealed class RecordingPresenter : IToastPresenter
    {
        public List<string> Messages { get; } = [];

        public TimeSpan LastDuration { get; private set; }

        public Task ShowAsync(string message, TimeSpan duration, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Messages.Add(message);
            LastDuration = duration;
            return Task.CompletedTask;
        }
    }
}
