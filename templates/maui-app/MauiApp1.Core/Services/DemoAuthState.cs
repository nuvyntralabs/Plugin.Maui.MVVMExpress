using System.ComponentModel;
using Plugin.Maui.MVVMExpress.Auth;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace MauiApp1;

/// <summary>In-memory demo session. Production apps should adapt Plugin.Maui.SecureSession.</summary>
public sealed class DemoAuthState : IAuthState, INotifyPropertyChanged
{
    public const string DemoEmail = "demo@mvvmexpress.dev";
    public const string DemoPassword = "secret";

    public bool IsAuthenticated { get; private set; }

    public string? UserName { get; private set; }

    public string? Email { get; private set; }

    public string? DisplayName => UserName;

    public event EventHandler? Changed;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Task<Result> SignInAsync(string userName, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            return Task.FromResult(Result.Failure("E_VAL", "Email and password are required"));
        }

        if (password != DemoPassword)
        {
            return Task.FromResult(Result.Failure("E_AUTH", "Invalid credentials"));
        }

        IsAuthenticated = true;
        UserName = userName.Trim();
        Email = UserName.Contains('@', StringComparison.Ordinal) ? UserName : null;
        RaiseSession();
        return Task.FromResult(Result.Success());
    }

    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IsAuthenticated = false;
        UserName = null;
        Email = null;
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
