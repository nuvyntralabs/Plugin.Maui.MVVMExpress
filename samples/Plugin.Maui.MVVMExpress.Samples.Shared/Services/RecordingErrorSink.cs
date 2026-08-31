using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Outcome;

namespace Plugin.Maui.MVVMExpress.Samples.Services;

public sealed class RecordingErrorSink : IErrorSink
{
    private readonly List<ErrorInfo> _errors = [];

    public IReadOnlyList<ErrorInfo> Errors => _errors;

    public Task HandleAsync(ErrorInfo error, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(error);
        cancellationToken.ThrowIfCancellationRequested();
        _errors.Add(error);
        return Task.CompletedTask;
    }
}
