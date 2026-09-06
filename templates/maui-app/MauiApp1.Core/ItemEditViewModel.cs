using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Forms;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace MauiApp1;

[RegisterViewModel]
[Route("edit")]
public sealed class ItemEditViewModel : FormViewModel
{
    private readonly IItemStore _store;
    private readonly FormField<string> _name;

    public ItemEditViewModel(IItemStore store, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
        _name = Field("Name", "");
        SaveCommand = new AsyncModelCommand(SaveAsync, () => !string.IsNullOrWhiteSpace(Name));
        Bind(_name, nameof(Name), () => SaveCommand.NotifyCanExecuteChanged());
    }

    public string Name
    {
        get => _name.Value ?? "";
        set => _name.Value = value;
    }

    public AsyncModelCommand SaveCommand { get; }

    private Task SaveAsync(CancellationToken cancellationToken)
        => SubmitAsync(
            async ct =>
            {
                await _store.SaveAsync(new Item(Guid.NewGuid().ToString("N"), Name.Trim()), ct).ConfigureAwait(false);
                return Result.Success();
            },
            cancellationToken: cancellationToken);
}
