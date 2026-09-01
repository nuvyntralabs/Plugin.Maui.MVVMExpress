namespace Plugin.Maui.MVVMExpress.Input;

/// <summary>Shared debounce / throttle / queue gate used by async commands.</summary>
internal sealed class CommandPipeline
{
    private readonly AsyncCommandOptions _options;
    private readonly SemaphoreSlim _queue = new(1, 1);
    private readonly object _gate = new();
    private CancellationTokenSource? _debounce;
    private DateTimeOffset _lastStart;

    public CommandPipeline(AsyncCommandOptions options)
    {
        _options = options;
    }

    public bool AllowsExecuteWhileRunning => _options.Concurrency is not ConcurrencyMode.Prevent;

    public bool InterruptsPrevious =>
        _options.Concurrency is ConcurrencyMode.CancelPrevious or ConcurrencyMode.Replace;

    public bool AllowsOverlap => _options.Concurrency == ConcurrencyMode.Allow;

    public bool Queues => _options.Concurrency == ConcurrencyMode.Queue;

    public async Task<bool> WaitPolicyAsync(CancellationToken cancellationToken)
    {
        if (_options.Throttle is { } throttle && throttle > TimeSpan.Zero)
        {
            lock (_gate)
            {
                var now = DateTimeOffset.UtcNow;
                if (now - _lastStart < throttle)
                {
                    return false;
                }

                _lastStart = now;
            }
        }

        if (_options.Debounce is not { } debounce || debounce <= TimeSpan.Zero)
        {
            return true;
        }

        CancellationTokenSource debounceCts;
        lock (_gate)
        {
            _debounce?.Cancel();
            _debounce?.Dispose();
            debounceCts = new CancellationTokenSource();
            _debounce = debounceCts;
        }

        try
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, debounceCts.Token);
            await Task.Delay(debounce, linked.Token).ConfigureAwait(false);
            return true;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return false;
        }
    }

    public Task EnterQueueAsync(CancellationToken cancellationToken)
        => Queues ? _queue.WaitAsync(cancellationToken) : Task.CompletedTask;

    public void ExitQueue()
    {
        if (Queues)
        {
            _queue.Release();
        }
    }
}
