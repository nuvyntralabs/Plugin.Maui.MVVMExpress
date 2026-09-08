using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.ChatHost;
using Plugin.Maui.MVVMExpress.Samples.Computed;
using Plugin.Maui.MVVMExpress.Samples.EscapeHatch;
using Plugin.Maui.MVVMExpress.Samples.Modals;

namespace Plugin.Maui.MVVMExpress.Samples.Playground;

[RegisterViewModel]
public partial class PlaygroundHomeViewModel : PageViewModel
{
    public PlaygroundHomeViewModel(INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
    }

    [AsyncModelCommand]
    private Task OpenCommandAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<PlaygroundCommandViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenDetailsAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<PlaygroundDetailsViewModel, PlaygroundDetailsArgs>(
            new PlaygroundDetailsArgs("From home"),
            cancellationToken);

    [AsyncModelCommand]
    private Task OpenDialogAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<PlaygroundDialogViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenFormAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<PlaygroundFormViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenAuthAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<PlaygroundSecureViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenListAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<PlaygroundListViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenChatAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<ChatHostViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenComputedAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<ComputedNameViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenManualAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<ManualCounterViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenModalAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<ModalHostViewModel>(cancellationToken);
}
