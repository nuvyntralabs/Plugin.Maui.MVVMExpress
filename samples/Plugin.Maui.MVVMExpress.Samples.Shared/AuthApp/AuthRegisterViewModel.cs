using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Forms;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Outcome;

namespace Plugin.Maui.MVVMExpress.Samples.AuthApp;

[RegisterViewModel]
[Route("register")]
public sealed class AuthRegisterViewModel : FormViewModel
{
    private readonly IAccountService _accounts;
    private readonly IAuthState _auth;
    private readonly FormField<string> _email;
    private readonly FormField<string> _password;
    private readonly FormField<string> _confirm;

    public AuthRegisterViewModel(IAccountService accounts, IAuthState auth, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(accounts);
        ArgumentNullException.ThrowIfNull(auth);
        _accounts = accounts;
        _auth = auth;
        _email = Field("Email", "");
        _password = Field("Password", "");
        _confirm = Field("Confirm", "");
        SubmitCommand = new AsyncModelCommand(SubmitCoreAsync);
        BackCommand = new AsyncModelCommand(ct => Navigator!.GoBackAsync(ct));
    }

    public string Email
    {
        get => _email.Value ?? "";
        set => _email.Value = value;
    }

    public string Password
    {
        get => _password.Value ?? "";
        set => _password.Value = value;
    }

    public string Confirm
    {
        get => _confirm.Value ?? "";
        set => _confirm.Value = value;
    }

    public AsyncModelCommand SubmitCommand { get; }

    public AsyncModelCommand BackCommand { get; }

    private Task SubmitCoreAsync(CancellationToken cancellationToken)
    {
        var mismatch = MustMatch(_password, _confirm, "Passwords do not match.");
        return SubmitAsync(async ct =>
        {
            if (mismatch is not null)
            {
                _confirm.SetErrors([mismatch]);
                return Plugin.Maui.MVVMExpress.Outcome.Outcome.Failure("E_VAL", mismatch.Message);
            }

            var created = await _accounts.RegisterAsync(Email, Password, cancellationToken: ct).ConfigureAwait(false);
            if (!created.IsSuccess)
            {
                return created;
            }

            var signedIn = await _auth.SignInAsync(Email, Password, ct).ConfigureAwait(false);
            if (!signedIn.IsSuccess)
            {
                return signedIn;
            }

            return await Navigator!.ResetAsync<AuthHomeViewModel>(ct).ConfigureAwait(false);
        }, mismatch is null ? null : [mismatch], cancellationToken);
    }
}
