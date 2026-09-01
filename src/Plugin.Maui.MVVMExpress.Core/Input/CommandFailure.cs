using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Outcome;
using Plugin.Maui.MVVMExpress.Threading;

namespace Plugin.Maui.MVVMExpress.Input;

internal static class CommandFailure
{
    public static async Task HandleAsync(Exception exception, AsyncCommandOptions options)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(options);
        var error = new ErrorInfo("E_CMD", exception.Message, exception);
        var sink = options.ErrorSink ?? NullErrorSink.Instance;
        try
        {
            await sink.HandleAsync(error).ConfigureAwait(false);
        }
        catch (Exception sinkException)
        {
            NotificationMarshaller.Diagnostics?.Trace("command", sinkException.Message);
        }

        if (options.Dialogs is { } dialogs)
        {
            try
            {
                await dialogs.ErrorAsync(error).ConfigureAwait(false);
            }
            catch (Exception dialogException)
            {
                NotificationMarshaller.Diagnostics?.Trace("command", dialogException.Message);
            }
        }
    }
}
