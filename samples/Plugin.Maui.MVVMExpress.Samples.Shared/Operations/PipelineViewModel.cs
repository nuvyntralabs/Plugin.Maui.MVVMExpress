using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Operations;

namespace Plugin.Maui.MVVMExpress.Samples.Operations;

/// <summary>Phase 8: <see cref="IOperationExecutor"/> timeout / retry / Outcome pipeline.</summary>
[RegisterViewModel]
[Route("pipeline")]
public sealed class PipelineViewModel : ViewModel
{
    private readonly IOperationExecutor _ops;
    private int _runs;
    private string _last = "idle";

    /// <summary>Creates the pipeline demo.</summary>
    public PipelineViewModel(IOperationExecutor ops)
    {
        ArgumentNullException.ThrowIfNull(ops);
        _ops = ops;
        RunCommand = new AsyncModelCommand(RunAsync);
    }

    /// <summary>Successful pipeline runs.</summary>
    public int Runs
    {
        get => _runs;
        private set => SetProperty(ref _runs, value);
    }

    /// <summary>Last Outcome code or <c>ok</c>.</summary>
    public string Last
    {
        get => _last;
        private set => SetProperty(ref _last, value);
    }

    /// <summary>Runs one successful operation.</summary>
    public AsyncModelCommand RunCommand { get; }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        var result = await _ops.RunAsync(
            async ct =>
            {
                await Task.Yield();
                ct.ThrowIfCancellationRequested();
                return Interlocked.Increment(ref _runs);
            },
            cancellationToken: cancellationToken).ConfigureAwait(false);
        Notify(nameof(Runs));
        Last = result.IsSuccess ? "ok" : result.Error?.Code ?? "fail";
    }
}
