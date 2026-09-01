using Plugin.Maui.MVVMExpress.Busy;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Forms;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Models;
using Plugin.Maui.MVVMExpress.Samples.Services;
using Plugin.Maui.MVVMExpress.Validation;

namespace Plugin.Maui.MVVMExpress.Samples.Crud;

public sealed class ProductEditViewModel : FormViewModel
{
    private readonly IProductCatalog _catalog;
    private readonly IErrorSink _errors;
    private readonly IBusyGate _busy;
    private readonly IValidator _validator;
    private readonly FormField<string> _name;
    private readonly FormField<decimal> _price;
    private Op.Outcome<Product>? _lastSave;

    public ProductEditViewModel(
        IProductCatalog catalog,
        IErrorSink errors,
        IBusyGate busy,
        IValidator? validator = null,
        INavigator? navigator = null,
        IDialogs? dialogs = null)
        : base(navigator, dialogs, errors, busy)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(busy);
        _catalog = catalog;
        _errors = errors;
        _busy = busy;
        _validator = validator ?? DataAnnotationsValidator.Instance;
        SaveCommand = new AsyncModelCommand(SaveAsync, () => !string.IsNullOrWhiteSpace(Name));
        _name = Field("Name", "");
        _price = Field("Price", 0m);
        _name.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(FormField<string>.Value))
            {
                Notify(nameof(Name));
                SaveCommand.NotifyCanExecuteChanged();
            }
        };
        _price.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(FormField<decimal>.Value))
            {
                Notify(nameof(Price));
            }
        };
    }

    public int Id { get; set; }

    public string Name
    {
        get => _name.Value ?? "";
        set => _name.Value = value ?? "";
    }

    public decimal Price
    {
        get => _price.Value;
        set => _price.Value = value;
    }

    public Op.Outcome<Product>? LastSave
    {
        get => _lastSave;
        private set => SetProperty(ref _lastSave, value);
    }

    public AsyncModelCommand SaveCommand { get; }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        using (_busy.Enter())
        {
            Status = State.ViewModelStatus.Saving;
            try
            {
                var product = new Product { Id = Id, Name = Name.Trim(), Price = Price };
                var validation = _validator.Validate(product);
                if (!validation.IsValid)
                {
                    Status = State.ViewModelStatus.Error;
                    LastSave = Op.Outcome<Product>.Failure(new Op.ErrorInfo("E_VALIDATION", validation.ToString()));
                    await _errors.HandleAsync(LastSave.Value.Error!, cancellationToken).ConfigureAwait(false);
                    return;
                }

                LastSave = await _catalog.SaveAsync(product, cancellationToken).ConfigureAwait(false);
                if (LastSave.Value.IsSuccess && LastSave.Value.Value is { } saved)
                {
                    Id = saved.Id;
                    Name = saved.Name;
                    Price = saved.Price;
                    MarkClean();
                    Status = State.ViewModelStatus.Success;
                    return;
                }

                Status = State.ViewModelStatus.Error;
                if (LastSave.Value.Error is { } error)
                {
                    await _errors.HandleAsync(error, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                Status = State.ViewModelStatus.Cancelled;
                throw;
            }
            catch (Exception ex)
            {
                Status = State.ViewModelStatus.Error;
                await _errors.HandleAsync(new Op.ErrorInfo("E_SAVE", ex.Message, ex), cancellationToken).ConfigureAwait(false);
                throw;
            }
        }
    }
}
