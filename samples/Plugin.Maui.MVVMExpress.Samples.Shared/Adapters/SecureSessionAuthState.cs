using Plugin.Maui.MVVMExpress.Auth;

namespace Plugin.Maui.MVVMExpress.Samples.Adapters;

/// <summary>
/// Intended production <see cref="IAuthState"/> adapter for
/// <c>Plugin.Maui.SecureSession</c>. Copy this type into an app that references that package
/// and replace <see cref="ISecureSessionPort"/> with the real session service.
/// </summary>
public sealed class SecureSessionAuthState : IAuthState
{
    private readonly ISecureSessionPort _session;

    /// <summary>Creates the adapter.</summary>
    public SecureSessionAuthState(ISecureSessionPort session)
    {
        ArgumentNullException.ThrowIfNull(session);
        _session = session;
        _session.Changed += (_, _) => Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public bool IsAuthenticated => _session.HasAccessToken;

    /// <inheritdoc />
    public string? UserName => _session.UserName;

    /// <inheritdoc />
    public string? Email => _session.Email;

    /// <inheritdoc />
    public string? DisplayName => _session.DisplayName ?? _session.UserName;

    /// <inheritdoc />
    public event EventHandler? Changed;

    /// <inheritdoc />
    public async Task<Op.Outcome> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        var result = await _session.SignInAsync(userName, password, cancellationToken).ConfigureAwait(false);
        Changed?.Invoke(this, EventArgs.Empty);
        return result;
    }

    /// <inheritdoc />
    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await _session.SignOutAsync(cancellationToken).ConfigureAwait(false);
        Changed?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>Port so the sample compiles without a SecureSession package reference.</summary>
public interface ISecureSessionPort
{
    /// <summary>Whether an access token is present.</summary>
    bool HasAccessToken { get; }

    /// <summary>User name claim.</summary>
    string? UserName { get; }

    /// <summary>Email claim.</summary>
    string? Email { get; }

    /// <summary>Display name claim.</summary>
    string? DisplayName { get; }

    /// <summary>Session change.</summary>
    event EventHandler? Changed;

    /// <summary>Signs in and stores tokens.</summary>
    Task<Op.Outcome> SignInAsync(string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>Clears tokens.</summary>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}
