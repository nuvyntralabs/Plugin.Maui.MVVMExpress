using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.AuthApp;

[RegisterViewModel]
[Route("//home")]
[RequiresAuth]
public sealed class AuthHomeViewModel : PageViewModel
{
    private readonly IAuthState _auth;
    private string _email = "";

    public AuthHomeViewModel(IAuthState auth, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(auth);
        _auth = auth;
        _auth.Changed += (_, _) => Refresh();
        SignOutCommand = new AsyncModelCommand(SignOutAsync);
        Refresh();
    }

    public string Email
    {
        get => _email;
        private set => SetProperty(ref _email, value);
    }

    public AsyncModelCommand SignOutCommand { get; }

    public override Task OnAppearingAsync(CancellationToken cancellationToken = default)
    {
        Refresh();
        return Task.CompletedTask;
    }

    private void Refresh() => Email = _auth.Email ?? _auth.DisplayName ?? _auth.UserName ?? "";

    private async Task SignOutAsync(CancellationToken cancellationToken)
    {
        if (!await Dialogs!.ConfirmAsync("Sign out?", "Return to login.", "Sign out", "Stay", cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        await _auth.SignOutAsync(cancellationToken).ConfigureAwait(false);
        await TrackNavigation(await Navigator!.ResetAsync<AuthLoginViewModel>(cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
