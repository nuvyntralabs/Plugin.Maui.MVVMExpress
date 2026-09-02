using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

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
}
