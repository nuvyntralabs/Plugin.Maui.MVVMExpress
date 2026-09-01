namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>Host options for <c>UseMvvmExpress</c>.</summary>
public sealed class MvvmExpressOptions
{
    private readonly List<Action<global::Microsoft.Extensions.DependencyInjection.IServiceCollection>> _registrations = [];

    /// <summary>When <see langword="true"/>, page disappear cancels the ViewModel token via the lifecycle behavior.</summary>
    public bool CancelOperationsOnDisappear { get; set; }

    /// <summary>When <see langword="true"/> in Debug, lifecycle and thread-hop traces are written. Ignored in Release.</summary>
    public bool EnableDiagnostics { get; set; }

    /// <summary>When <see langword="true"/> (default), property and command notifications hop to <c>IMainThread</c>.</summary>
    public bool MarshalNotifications { get; set; } = true;

    /// <summary>When <see langword="true"/> (default), <c>ViewModelLifecycleBehavior</c> is attached to pages automatically.</summary>
    public bool AutoAttachLifecycle { get; set; } = true;

    /// <summary>When <see langword="true"/> (default), dirty forms confirm via <c>IDialogs</c> before leaving.</summary>
    public bool ConfirmDirtyNavigation { get; set; } = true;

    /// <summary>When <see langword="true"/> (default), failed navigation outcomes are forwarded to <c>IErrorSink</c> / <c>IDialogs</c>.</summary>
    public bool ForwardNavigationFailures { get; set; } = true;

    /// <summary>When <see langword="true"/> (default), generated <c>[Route]</c> / <c>[RequiresAuth]</c> registrations are applied.</summary>
    public bool ApplyGeneratedRegistrations { get; set; } = true;

    /// <summary>Adds a service-registration callback run at the end of <c>UseMvvmExpress</c>.</summary>
    public MvvmExpressOptions AddRegistration(Action<global::Microsoft.Extensions.DependencyInjection.IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        _registrations.Add(configure);
        return this;
    }

    internal void ApplyRegistrations(global::Microsoft.Extensions.DependencyInjection.IServiceCollection services)
    {
        foreach (var configure in _registrations)
        {
            configure(services);
        }
    }
}
