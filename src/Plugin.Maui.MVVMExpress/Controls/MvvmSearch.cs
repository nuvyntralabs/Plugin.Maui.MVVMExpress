namespace Plugin.Maui.MVVMExpress.Controls;

/// <summary>
/// Search field over an <see cref="Entry"/>. Bind <see cref="Text"/> to <c>SearchQuery.Text</c>.
/// Android <c>SearchBar</c> is not required.
/// </summary>
public sealed class MvvmSearch : ContentView
{
    /// <summary>Search text.</summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(MvvmSearch),
        defaultValue: "",
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: OnTextChanged);

    /// <summary>Placeholder.</summary>
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(MvvmSearch),
        defaultValue: "Search",
        propertyChanged: OnPlaceholderChanged);

    private readonly Entry _entry = new() { Placeholder = "Search" };
    private bool _suppress;

    /// <summary>Creates the control.</summary>
    public MvvmSearch()
    {
        Content = _entry;
        _entry.TextChanged += OnEntryTextChanged;
    }

    /// <summary>Gets or sets the search text. Bind to <c>SearchQuery.Text</c>.</summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Gets or sets the placeholder.</summary>
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_suppress)
        {
            return;
        }

        _suppress = true;
        try
        {
            Text = e.NewTextValue ?? "";
        }
        finally
        {
            _suppress = false;
        }
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MvvmSearch { _suppress: false } search)
        {
            search._suppress = true;
            try
            {
                search._entry.Text = newValue as string ?? "";
            }
            finally
            {
                search._suppress = false;
            }
        }
    }

    private static void OnPlaceholderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MvvmSearch search)
        {
            search._entry.Placeholder = newValue as string ?? "";
        }
    }
}
