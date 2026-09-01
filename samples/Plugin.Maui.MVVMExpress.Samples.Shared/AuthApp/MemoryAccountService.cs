using Plugin.Maui.MVVMExpress.Auth;

namespace Plugin.Maui.MVVMExpress.Samples.AuthApp;

public sealed class MemoryAccountService : IAccountService
{
    private readonly Dictionary<string, string> _passwords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["demo@mvvmexpress.dev"] = "secret"
    };

    public Task<Op.Outcome> RegisterAsync(
        string email,
        string password,
        string? displayName = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(Op.Outcome.Failure("E_VAL", "Email and password are required"));
        }

        if (!_passwords.TryAdd(email.Trim(), password))
        {
            return Task.FromResult(Op.Outcome.Failure("E_EXISTS", "An account already exists"));
        }

        return Task.FromResult(Op.Outcome.Success());
    }

    public Task<Op.Outcome> ResetPasswordAsync(string email, string newPassword, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
        {
            return Task.FromResult(Op.Outcome.Failure("E_VAL", "Email and password are required"));
        }

        if (!_passwords.ContainsKey(email.Trim()))
        {
            return Task.FromResult(Op.Outcome.Failure("E_NOTFOUND", "Unknown account"));
        }

        _passwords[email.Trim()] = newPassword;
        return Task.FromResult(Op.Outcome.Success());
    }
}
