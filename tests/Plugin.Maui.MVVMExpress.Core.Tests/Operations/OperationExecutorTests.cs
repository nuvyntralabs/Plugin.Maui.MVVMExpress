using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Operations;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Operations;

public sealed class OperationExecutorTests
{
    [Fact]
    public async Task Run_Success()
    {
        var executor = new OperationExecutor();
        var result = await executor.RunAsync(async ct =>
        {
            await Task.Yield();
            return 7;
        });
        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Value);
    }

    [Fact]
    public async Task Run_Failure_GoesToOutcome()
    {
        var executor = new OperationExecutor();
        var result = await executor.RunAsync<int>(_ => throw new InvalidOperationException("boom"));
        Assert.False(result.IsSuccess);
        Assert.Equal("E_OP", result.Error?.Code);
    }

    [Fact]
    public async Task Retry_ThenSuccess()
    {
        var attempts = 0;
        var executor = new OperationExecutor();
        var result = await executor.RunAsync(_ =>
        {
            attempts++;
            if (attempts < 3)
            {
                throw new InvalidOperationException("retry");
            }

            return Task.FromResult(1);
        }, new OperationOptions { RetryCount = 2, RetryDelay = TimeSpan.Zero });
        Assert.True(result.IsSuccess);
        Assert.Equal(3, attempts);
    }

    [Fact]
    public async Task Prevent_SecondRun_FailsBusy()
    {
        var started = new TaskCompletionSource();
        var release = new TaskCompletionSource();
        var executor = new OperationExecutor();
        var first = executor.RunAsync(async ct =>
        {
            started.TrySetResult();
            await release.Task.WaitAsync(ct);
        });
        await started.Task;
        var second = await executor.RunAsync(_ => Task.CompletedTask);
        Assert.Equal("E_BUSY", second.Error?.Code);
        release.TrySetResult();
        Assert.True((await first).IsSuccess);
    }

    [Fact]
    public async Task Debounce_SupersedesEarlierCall()
    {
        var runs = 0;
        var executor = new OperationExecutor();
        var options = new OperationOptions { Debounce = TimeSpan.FromMilliseconds(30) };
        var first = executor.RunAsync(_ =>
        {
            Interlocked.Increment(ref runs);
            return Task.CompletedTask;
        }, options);
        var second = executor.RunAsync(_ =>
        {
            Interlocked.Increment(ref runs);
            return Task.CompletedTask;
        }, options);
        await Task.WhenAll(first, second);
        Assert.Equal(1, runs);
    }

    [Fact]
    public async Task Queue_RunsSecondAfterFirst()
    {
        var order = new List<int>();
        var started = new TaskCompletionSource();
        var release = new TaskCompletionSource();
        var executor = new OperationExecutor();
        var options = new OperationOptions { Concurrency = ConcurrencyMode.Queue };
        var first = executor.RunAsync(async ct =>
        {
            order.Add(1);
            started.TrySetResult();
            await release.Task.WaitAsync(ct);
        }, options);
        await started.Task;
        var second = executor.RunAsync(_ =>
        {
            order.Add(2);
            return Task.CompletedTask;
        }, options);
        release.TrySetResult();
        Assert.True((await first).IsSuccess);
        Assert.True((await second).IsSuccess);
        Assert.Equal([1, 2], order);
    }
}
