namespace Plugin.Maui.MVVMExpress.Threading;

/// <summary>MAUI <see cref="MainThread"/> adapter.</summary>
public sealed class MauiMainThread : IMainThread
{
    /// <inheritdoc />
    public bool IsMainThread => MainThread.IsMainThread;

    /// <inheritdoc />
    public void BeginInvoke(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (MainThread.IsMainThread)
        {
            action();
            return;
        }

        MainThread.BeginInvokeOnMainThread(action);
    }

    /// <inheritdoc />
    public Task InvokeAsync(Action action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        cancellationToken.ThrowIfCancellationRequested();
        return MainThread.InvokeOnMainThreadAsync(action);
    }

    /// <inheritdoc />
    public Task InvokeAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        cancellationToken.ThrowIfCancellationRequested();
        return MainThread.InvokeOnMainThreadAsync(action);
    }
}
