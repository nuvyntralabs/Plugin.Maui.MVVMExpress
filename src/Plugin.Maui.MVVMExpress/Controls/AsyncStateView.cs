using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Controls;

/// <summary>Simple loading / error / value presenter for <see cref="AsyncState{T}"/>.</summary>
public sealed class AsyncStateView : ContentView
{
    /// <summary>Bound async state.</summary>
    public static readonly BindableProperty StateProperty = BindableProperty.Create(
        nameof(State),
        typeof(object),
        typeof(AsyncStateView),
        propertyChanged: OnStateChanged);

    /// <summary>Template used when the state is loading.</summary>
    public static readonly BindableProperty LoadingTemplateProperty = BindableProperty.Create(
        nameof(LoadingTemplate),
        typeof(DataTemplate),
        typeof(AsyncStateView));

    /// <summary>Template used when the state has an error.</summary>
    public static readonly BindableProperty ErrorTemplateProperty = BindableProperty.Create(
        nameof(ErrorTemplate),
        typeof(DataTemplate),
        typeof(AsyncStateView));

    /// <summary>Template used when the state has a value.</summary>
    public static readonly BindableProperty ValueTemplateProperty = BindableProperty.Create(
        nameof(ValueTemplate),
        typeof(DataTemplate),
        typeof(AsyncStateView));

    /// <summary>Gets or sets the async state object (<see cref="AsyncState{T}"/>).</summary>
    public object? State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>Loading template.</summary>
    public DataTemplate? LoadingTemplate
    {
        get => (DataTemplate?)GetValue(LoadingTemplateProperty);
        set => SetValue(LoadingTemplateProperty, value);
    }

    /// <summary>Error template.</summary>
    public DataTemplate? ErrorTemplate
    {
        get => (DataTemplate?)GetValue(ErrorTemplateProperty);
        set => SetValue(ErrorTemplateProperty, value);
    }

    /// <summary>Value template. Binding context is the state's <c>Data</c>.</summary>
    public DataTemplate? ValueTemplate
    {
        get => (DataTemplate?)GetValue(ValueTemplateProperty);
        set => SetValue(ValueTemplateProperty, value);
    }

    private static void OnStateChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        var view = (AsyncStateView)bindable;
        if (oldValue is System.ComponentModel.INotifyPropertyChanged oldNpc)
        {
            oldNpc.PropertyChanged -= view.OnStatePropertyChanged;
        }

        if (newValue is System.ComponentModel.INotifyPropertyChanged npc)
        {
            npc.PropertyChanged += view.OnStatePropertyChanged;
        }

        view.Refresh();
    }

    private void OnStatePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => Refresh();

    private void Refresh()
    {
        if (State is null)
        {
            Content = null;
            return;
        }

        var isLoading = ReadBool(State, "IsLoading");
        var hasError = ReadBool(State, "HasError");
        DataTemplate? template;
        object? context = State;
        if (isLoading)
        {
            template = LoadingTemplate;
        }
        else if (hasError)
        {
            template = ErrorTemplate;
        }
        else
        {
            template = ValueTemplate;
            context = Read(State, "Data") ?? State;
        }

        if (template is null)
        {
            Content = new Label { Text = hasError ? Read(State, "Error")?.ToString() ?? "Error" : isLoading ? "Loading…" : context?.ToString() };
            return;
        }

        var content = template.CreateContent();
        if (content is View view)
        {
            view.BindingContext = context;
            Content = view;
        }
    }

    private static bool ReadBool(object instance, string name)
        => Read(instance, name) is true;

    private static object? Read(object instance, string name)
        => instance.GetType().GetProperty(name)?.GetValue(instance);
}
