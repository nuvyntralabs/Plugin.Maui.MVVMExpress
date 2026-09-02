using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Forms;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Playground;

[RegisterViewModel]
public sealed class PlaygroundFormViewModel : FormViewModel
{
    private readonly FormField<string> _title;

    public PlaygroundFormViewModel(INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        _title = Field("Title", "Draft");
        SaveCommand = new AsyncModelCommand(SaveAsync, () => !string.IsNullOrWhiteSpace(Title));
        Bind(_title, nameof(Title), () => SaveCommand.NotifyCanExecuteChanged());
    }

    public string Title
    {
        get => _title.Value ?? "";
        set => _title.Value = value;
    }

    public AsyncModelCommand SaveCommand { get; }

    private Task SaveAsync(CancellationToken cancellationToken)
        => SubmitAsync(_ => Task.FromResult(Op.Outcome.Success()), cancellationToken: cancellationToken);
}
