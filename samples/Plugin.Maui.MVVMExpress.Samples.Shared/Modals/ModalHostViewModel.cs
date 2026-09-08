using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Modals;

/// <summary>Phase 9: pushes <see cref="NoteModalViewModel"/> onto the modal stack.</summary>
[RegisterViewModel]
[Route("modal")]
public sealed class ModalHostViewModel : PageViewModel
{
    /// <summary>Creates the host.</summary>
    public ModalHostViewModel(IPageNavigator pages)
        : base(pages)
    {
        Pages = pages;
        OpenModalCommand = new AsyncModelCommand(ct => Pages.PushModalAsync<NoteModalViewModel>(ct));
    }

    /// <summary>Page navigator.</summary>
    public IPageNavigator Pages { get; }

    /// <summary>Pushes the note modal.</summary>
    public AsyncModelCommand OpenModalCommand { get; }

    /// <summary>Current modal depth.</summary>
    public int ModalCount => Pages.ModalStack.Count;
}
