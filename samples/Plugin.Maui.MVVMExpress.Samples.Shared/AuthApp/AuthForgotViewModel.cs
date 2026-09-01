using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Forms;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Outcome;

namespace Plugin.Maui.MVVMExpress.Samples.AuthApp;

[RegisterViewModel]
[Route("forgot")]
public sealed class AuthForgotViewModel : FormViewModel
{
    private readonly IAccountService _accounts;
    private readonly FormField<string> _email;
    private readonly FormField<string> _password;
    private readonly FormField<string> _confirm;

    public AuthForgotViewModel(IAccountService accounts, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(accounts);
        _accounts = accounts;
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

            var reset = await _accounts.ResetPasswordAsync(Email, Password, ct).ConfigureAwait(false);
            if (!reset.IsSuccess)
            {
                return reset;
            }

            return await Navigator!.GoBackAsync(ct).ConfigureAwait(false);
        }, mismatch is null ? null : [mismatch], cancellationToken);
    }
}
