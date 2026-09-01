using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Dialogs;

/// <summary>MAUI <see cref="INotifier"/> that shows a real toast overlay.</summary>
public sealed class MauiNotifier : INotifier
{
    private readonly IToastPresenter _presenter;

    /// <summary>Creates a notifier. Tests inject <paramref name="presenter"/>.</summary>
    public MauiNotifier(IToastPresenter? presenter = null, IWindowContext? window = null)
        => _presenter = presenter ?? new MauiToastPresenter(window);

    /// <inheritdoc />
    public Task ToastAsync(string message, TimeSpan? duration = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        cancellationToken.ThrowIfCancellationRequested();
        return _presenter.ShowAsync(message, duration ?? TimeSpan.FromSeconds(2), cancellationToken);
    }
}
