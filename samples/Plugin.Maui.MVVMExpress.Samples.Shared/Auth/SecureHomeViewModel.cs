using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Auth;

public sealed class SecureHomeViewModel : PageViewModel
{
    public SecureHomeViewModel(IAuthState auth, INavigator navigator)
        : base(navigator)
    {
        ArgumentNullException.ThrowIfNull(auth);
        UserName = auth.UserName;
        SignOutCommand = new AsyncModelCommand(async ct =>
        {
            await auth.SignOutAsync(ct).ConfigureAwait(false);
            await Navigator!.GoBackAsync(ct).ConfigureAwait(false);
        });
    }

    public string? UserName { get; }

    public AsyncModelCommand SignOutCommand { get; }
}
