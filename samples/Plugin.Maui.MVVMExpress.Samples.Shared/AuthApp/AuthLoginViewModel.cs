using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.AuthApp;

[RegisterViewModel]
[Route("//login")]
public sealed class AuthLoginViewModel : PageViewModel
{
    private readonly IAuthState _auth;
    private string _email = "demo@mvvmexpress.dev";
    private string _password = "secret";

    public AuthLoginViewModel(IAuthState auth, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(auth);
        _auth = auth;
        SignInCommand = new AsyncModelCommand(SignInAsync);
        RegisterCommand = new AsyncModelCommand(ct => Navigator!.NavigateToAsync<AuthRegisterViewModel>(ct));
        ForgotCommand = new AsyncModelCommand(ct => Navigator!.NavigateToAsync<AuthForgotViewModel>(ct));
    }

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public AsyncModelCommand SignInCommand { get; }

    public AsyncModelCommand RegisterCommand { get; }

    public AsyncModelCommand ForgotCommand { get; }

    private async Task SignInAsync(CancellationToken cancellationToken)
    {
        var result = await _auth.SignInAsync(Email, Password, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            await Dialogs!.ErrorAsync(result.Error!, cancellationToken).ConfigureAwait(false);
            return;
        }

        await TrackNavigation(await Navigator!.ResetAsync<AuthHomeViewModel>(cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
