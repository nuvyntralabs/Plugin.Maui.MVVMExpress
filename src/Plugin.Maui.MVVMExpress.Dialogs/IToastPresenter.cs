namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>Shows a short-lived in-app toast. Tests inject a recording implementation.</summary>
public interface IToastPresenter
{
    /// <summary>Displays <paramref name="message"/> for <paramref name="duration"/>.</summary>
    Task ShowAsync(string message, TimeSpan duration, CancellationToken cancellationToken = default);
}
