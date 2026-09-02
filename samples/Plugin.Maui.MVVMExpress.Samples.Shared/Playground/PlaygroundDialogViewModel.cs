using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Playground;

[RegisterViewModel]
public partial class PlaygroundDialogViewModel : PageViewModel
{
    [Notify]
    private string _lastResult = "";

    public PlaygroundDialogViewModel(INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
    }

    [AsyncModelCommand]
    private async Task AlertAsync(CancellationToken cancellationToken)
    {
        await Dialogs!.AlertAsync("Playground", "Hello from IDialogs.", cancellationToken: cancellationToken).ConfigureAwait(false);
        LastResult = "Alert dismissed";
    }

    [AsyncModelCommand]
    private async Task ConfirmAsync(CancellationToken cancellationToken)
    {
        var ok = await Dialogs!.ConfirmAsync("Continue?", "This is IDialogs.ConfirmAsync.", cancellationToken: cancellationToken).ConfigureAwait(false);
        LastResult = ok ? "Confirmed" : "Cancelled";
    }
}
