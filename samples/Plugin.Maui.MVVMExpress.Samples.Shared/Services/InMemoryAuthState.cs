using System.ComponentModel;
using Plugin.Maui.MVVMExpress.Auth;

namespace Plugin.Maui.MVVMExpress.Samples.Services;

public sealed class InMemoryAuthState : IAuthState, IRoleState, INotifyPropertyChanged
{
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);

    public bool IsAuthenticated { get; private set; }

    public string? UserName { get; private set; }

    public string? Email { get; private set; }

    public string? DisplayName => UserName;

    public event EventHandler? Changed;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyCollection<string> Roles => _roles;

    public bool HasRole(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        return _roles.Contains(role);
    }

    public void SetRoles(params string[] roles)
    {
        ArgumentNullException.ThrowIfNull(roles);
        _roles.Clear();
        foreach (var role in roles)
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                _roles.Add(role);
            }
        }
    }

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
        Email = UserName.Contains('@', StringComparison.Ordinal) ? UserName : null;
        RaiseSession();
        return Task.FromResult(Op.Outcome.Success());
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IsAuthenticated = false;
        UserName = null;
        Email = null;
        _roles.Clear();
        RaiseSession();
        return Task.CompletedTask;
    }

    private void RaiseSession()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAuthenticated)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
