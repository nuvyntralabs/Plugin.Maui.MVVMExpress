using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace MauiApp1;

[RegisterViewModel]
[Route("login")]
public partial class LoginViewModel : PageViewModel
{
    private readonly IAuthState _auth;

    [Notify] private string _email = DemoAuthState.DemoEmail;
    [Notify] private string _password = DemoAuthState.DemoPassword;

    public LoginViewModel(IAuthState auth, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(auth);
        _auth = auth;
    }

    [AsyncModelCommand]
    private async Task SignInAsync(CancellationToken cancellationToken)
    {
        var result = await _auth.SignInAsync(Email, Password, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            await Dialogs!.ErrorAsync(result.Error!, cancellationToken).ConfigureAwait(false);
            return;
        }

        await TrackNavigation(await Navigator!.ResetAsync<MainPageViewModel>(cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
