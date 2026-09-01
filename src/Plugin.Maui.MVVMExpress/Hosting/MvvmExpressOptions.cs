namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>Host options for <c>UseMvvmExpress</c>.</summary>
public sealed class MvvmExpressOptions
{
    /// <summary>When <see langword="true"/>, page disappear cancels the ViewModel token via the lifecycle behavior.</summary>
    public bool CancelOperationsOnDisappear { get; set; }

    /// <summary>When <see langword="true"/> in Debug, lifecycle traces are written. Ignored in Release.</summary>
    public bool EnableDiagnostics { get; set; }
}
