namespace Plugin.Maui.MVVMExpress.Auth;

/// <summary>Authentication flag. Production apps should adapt Plugin.Maui.SecureSession.</summary>
public interface IAuthState
{
    /// <summary>Gets a value indicating whether a user is signed in.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Signed-in display name, if any.</summary>
    string? UserName { get; }

    /// <summary>Attempts sign-in.</summary>
    Task<Outcome.Outcome> SignInAsync(string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>Clears the session.</summary>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}
