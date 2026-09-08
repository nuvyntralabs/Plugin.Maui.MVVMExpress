using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Basic;

namespace Plugin.Maui.MVVMExpress.Sample.Compatibility;

/// <summary>Phase 8: keep CommunityToolkit <c>ObservableObject</c>, inject MVVMExpress services.</summary>
public sealed partial class ToolkitInboxViewModel : ObservableObject
{
    /// <summary>Creates the interop ViewModel.</summary>
    public ToolkitInboxViewModel(INavigator navigator, IDialogs dialogs)
    {
        Navigator = navigator;
        Dialogs = dialogs;
    }

    /// <summary>Typed navigator.</summary>
    public INavigator Navigator { get; }

    /// <summary>Dialogs.</summary>
    public IDialogs Dialogs { get; }

    /// <summary>Last dialog title.</summary>
    [ObservableProperty]
    private string _lastAlert = "";

    [RelayCommand]
    private Task OpenCounterAsync(CancellationToken cancellationToken)
        => Navigator.NavigateToAsync<CounterViewModel>(cancellationToken);

    [RelayCommand]
    private async Task WarnAsync(CancellationToken cancellationToken)
    {
        await Dialogs.AlertAsync("Toolkit", "CommunityToolkit ObservableObject + IDialogs", cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        LastAlert = "Toolkit";
    }
}
