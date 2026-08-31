using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Auth;

public sealed class LoginViewModel : PageViewModel
{
    private readonly IAuthState _auth;
    private string _userName = "";
    private string _password = "";
    private Op.Outcome? _lastResult;

    public LoginViewModel(IAuthState auth, INavigator navigator)
        : base(navigator)
    {
        ArgumentNullException.ThrowIfNull(auth);
        _auth = auth;
        SignInCommand = new AsyncModelCommand(SignInAsync, () => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password));
    }

    public string UserName
    {
        get => _userName;
        set
        {
            if (SetProperty(ref _userName, value))
            {
                SignInCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                SignInCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public Op.Outcome? LastResult
    {
        get => _lastResult;
        private set => SetProperty(ref _lastResult, value);
    }

    public AsyncModelCommand SignInCommand { get; }

    private async Task SignInAsync(CancellationToken cancellationToken)
    {
        LastResult = await _auth.SignInAsync(UserName, Password, cancellationToken).ConfigureAwait(false);
        if (LastResult.Value.IsSuccess)
        {
            LastResult = await Navigator!.NavigateToAsync<SecureHomeViewModel>(cancellationToken).ConfigureAwait(false);
        }
    }
}
