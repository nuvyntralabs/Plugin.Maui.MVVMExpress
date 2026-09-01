using System.ComponentModel;
using Plugin.Maui.MVVMExpress.Busy;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
namespace Plugin.Maui.MVVMExpress.Forms;

/// <summary>
/// Page ViewModel with tracked <see cref="FormField{T}"/> values, dirty navigation guard, and undo / redo.
/// </summary>
public abstract class FormViewModel : PageViewModel, IDirtyState
{
    private readonly List<IFormField> _fields = [];
    private readonly UndoStack _history = new();
    private readonly Dictionary<IFormField, object?> _changing = [];
    private bool _suppressHistory;
    private bool _isDirty;

    /// <summary>Creates a form ViewModel.</summary>
    protected FormViewModel(
        INavigator? navigator = null,
        IDialogs? dialogs = null,
        IErrorSink? errors = null,
        IBusyGate? busy = null)
        : base(navigator, dialogs, errors, busy)
    {
        ResetCommand = new ModelCommand(Reset, () => IsDirty);
        UndoCommand = new ModelCommand(Undo, () => CanUndo);
        RedoCommand = new ModelCommand(Redo, () => CanRedo);
        _history.PropertyChanged += OnHistoryChanged;
    }

    /// <summary>Tracked fields.</summary>
    public IReadOnlyList<IFormField> Fields => _fields;

    /// <inheritdoc />
    public bool IsDirty
    {
        get => _isDirty;
        private set
        {
            if (SetProperty(ref _isDirty, value))
            {
                ResetCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>Gets a value indicating whether <see cref="Undo"/> has work.</summary>
    public bool CanUndo => _history.CanUndo;

    /// <summary>Gets a value indicating whether <see cref="Redo"/> has work.</summary>
    public bool CanRedo => _history.CanRedo;

    /// <summary>Restores accepted originals.</summary>
    public ModelCommand ResetCommand { get; }

    /// <summary>Undoes the last field edit.</summary>
    public ModelCommand UndoCommand { get; }

    /// <summary>Redoes the last undone edit.</summary>
    public ModelCommand RedoCommand { get; }

    /// <inheritdoc />
    public override Task<bool> CanNavigateAwayAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(!IsDirty);

    /// <inheritdoc />
    public void MarkClean()
    {
        foreach (var field in _fields)
        {
            field.MarkClean();
        }

        _history.Clear();
        RefreshDirty();
    }

    /// <inheritdoc />
    public void Reset()
    {
        _suppressHistory = true;
        try
        {
            foreach (var field in _fields)
            {
                field.Reset();
            }

            _history.Clear();
            RefreshDirty();
        }
        finally
        {
            _suppressHistory = false;
        }
    }

    /// <summary>Undoes the last recorded field change.</summary>
    public void Undo()
    {
        _suppressHistory = true;
        try
        {
            _history.Undo();
            RefreshDirty();
        }
        finally
        {
            _suppressHistory = false;
        }
    }

    /// <summary>Redoes the last undone field change.</summary>
    public void Redo()
    {
        _suppressHistory = true;
        try
        {
            _history.Redo();
            RefreshDirty();
        }
        finally
        {
            _suppressHistory = false;
        }
    }

    /// <summary>Tracks <paramref name="field"/> and records undo entries when its value changes.</summary>
    protected TField Track<TField>(TField field)
        where TField : class, IFormField
    {
        ArgumentNullException.ThrowIfNull(field);
        _fields.Add(field);
        field.PropertyChanging += OnFieldChanging;
        field.PropertyChanged += OnFieldChanged;
        RefreshDirty();
        return field;
    }

    /// <summary>Creates and tracks a <see cref="FormField{T}"/>.</summary>
    protected FormField<T> Field<T>(string name, T? original = default)
        => Track(new FormField<T>(name, original));

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _history.PropertyChanged -= OnHistoryChanged;
            foreach (var field in _fields)
            {
                field.PropertyChanging -= OnFieldChanging;
                field.PropertyChanged -= OnFieldChanged;
            }
        }

        base.Dispose(disposing);
    }

    private void OnFieldChanging(object? sender, PropertyChangingEventArgs e)
    {
        if (_suppressHistory || sender is not IFormField field || e.PropertyName != nameof(FormField<object>.Value))
        {
            return;
        }

        _changing[field] = field.BoxedValue;
    }

    private void OnFieldChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not IFormField field)
        {
            return;
        }

        if (e.PropertyName is nameof(IFormField.IsDirty) or nameof(FormField<object>.Value))
        {
            RefreshDirty();
        }

        if (_suppressHistory
            || e.PropertyName != nameof(FormField<object>.Value)
            || !_changing.Remove(field, out var previous))
        {
            return;
        }

        var next = field.BoxedValue;
        _history.Push(
            () => field.RestoreBoxed(previous),
            () => field.RestoreBoxed(next));
    }

    private void RefreshDirty()
    {
        IsDirty = _fields.Exists(static item => item.IsDirty);
    }

    private void OnHistoryChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(UndoStack.CanUndo))
        {
            Notify(nameof(CanUndo));
            UndoCommand.NotifyCanExecuteChanged();
        }
        else if (e.PropertyName is nameof(UndoStack.CanRedo))
        {
            Notify(nameof(CanRedo));
            RedoCommand.NotifyCanExecuteChanged();
        }
    }
}
