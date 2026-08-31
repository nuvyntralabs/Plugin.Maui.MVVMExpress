using Plugin.Maui.MVVMExpress.Auth;

namespace Plugin.Maui.MVVMExpress.Samples.Services;

public sealed class InMemoryAuthState : IAuthState
{
    public bool IsAuthenticated { get; private set; }

    public string? UserName { get; private set; }

    public Task<Op.Outcome> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(Op.Outcome.Failure("E_VAL", "User name and password are required"));
        }

        if (password != "secret")
        {
            return Task.FromResult(Op.Outcome.Failure("E_AUTH", "Invalid credentials"));
        }

        IsAuthenticated = true;
        UserName = userName.Trim();
        return Task.FromResult(Op.Outcome.Success());
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IsAuthenticated = false;
        UserName = null;
        return Task.CompletedTask;
    }
}
