using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Modals;

/// <summary>Phase 9: modal page popped with <see cref="IPageNavigator.PopModalAsync"/>.</summary>
[RegisterViewModel]
[Route("note-modal")]
public sealed class NoteModalViewModel : PageViewModel
{
    /// <summary>Creates the modal.</summary>
    public NoteModalViewModel(IPageNavigator pages)
        : base(pages)
    {
        Pages = pages;
        CloseCommand = new AsyncModelCommand(ct => Pages.PopModalAsync(ct));
    }

    /// <summary>Page navigator (modal stack).</summary>
    public IPageNavigator Pages { get; }

    /// <summary>Modal body.</summary>
    public string Body { get; } = "Pushed with PushModalAsync.";

    /// <summary>Pops this modal.</summary>
    public AsyncModelCommand CloseCommand { get; }
}
