using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;

namespace Plugin.Maui.MVVMExpress.Samples.Playground;

[RegisterViewModel]
public partial class PlaygroundCommandViewModel : PageViewModel
{
    [Notify]
    private int _count;

    [AsyncModelCommand]
    private async Task IncrementAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(80, cancellationToken).ConfigureAwait(false);
        Count++;
    }
}
