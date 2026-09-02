using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Playground;

[RegisterViewModel]
[RequiresAuth]
public sealed class PlaygroundSecureViewModel : PageViewModel
{
    private readonly IAuthState _auth;
    private string _email = "";

    public PlaygroundSecureViewModel(IAuthState auth, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(auth);
        _auth = auth;
        SignOutCommand = new AsyncModelCommand(SignOutAsync);
        Refresh();
    }

    public string Email
    {
        get => _email;
        private set => SetProperty(ref _email, value);
    }

    public AsyncModelCommand SignOutCommand { get; }

    /// <inheritdoc />
    public override Task OnAppearingAsync(CancellationToken cancellationToken = default)
    {
        Refresh();
        return Task.CompletedTask;
    }

    private void Refresh() => Email = _auth.Email ?? _auth.DisplayName ?? _auth.UserName ?? "";

    private async Task SignOutAsync(CancellationToken cancellationToken)
    {
        await _auth.SignOutAsync(cancellationToken).ConfigureAwait(false);
        await TrackNavigation(await Navigator!.ResetAsync<PlaygroundHomeViewModel>(cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
