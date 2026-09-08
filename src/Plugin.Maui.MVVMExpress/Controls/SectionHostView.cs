using System.Windows.Input;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Composition;

namespace Plugin.Maui.MVVMExpress.Controls;

/// <summary>
/// In-place tab host bound to <see cref="SectionHostViewModel.SelectCommand"/> and <see cref="ISectionHost.CurrentKey"/>.
/// Apps should not write visibility flippers in code-behind.
/// </summary>
public sealed class SectionHostView : ContentView
{
    /// <summary>Bound section host.</summary>
    public static readonly BindableProperty HostProperty = BindableProperty.Create(
        nameof(Host),
        typeof(ISectionHost),
        typeof(SectionHostView),
        propertyChanged: OnHostChanged);

    /// <summary>Current section key (mirrors <see cref="ISectionHost.CurrentKey"/>).</summary>
    public static readonly BindableProperty CurrentKeyProperty = BindableProperty.Create(
        nameof(CurrentKey),
        typeof(string),
        typeof(SectionHostView),
        defaultValue: "");

    /// <summary>Select command (mirrors <see cref="ISectionHost"/> when the host is a <see cref="SectionHostViewModel"/>).</summary>
    public static readonly BindableProperty SelectCommandProperty = BindableProperty.Create(
        nameof(SelectCommand),
        typeof(ICommand),
        typeof(SectionHostView));

    /// <summary>Current section ViewModel.</summary>
    public static readonly BindableProperty CurrentProperty = BindableProperty.Create(
        nameof(Current),
        typeof(IViewModel),
        typeof(SectionHostView));

    /// <summary>Gets or sets the section host.</summary>
    public ISectionHost? Host
    {
        get => (ISectionHost?)GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }

    /// <summary>Gets or sets the current section key.</summary>
    public string CurrentKey
    {
        get => (string)GetValue(CurrentKeyProperty);
        set => SetValue(CurrentKeyProperty, value);
    }

    /// <summary>Gets or sets the select command.</summary>
    public ICommand? SelectCommand
    {
        get => (ICommand?)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    /// <summary>Gets or sets the current section ViewModel.</summary>
    public IViewModel? Current
    {
        get => (IViewModel?)GetValue(CurrentProperty);
        set => SetValue(CurrentProperty, value);
    }

    private static void OnHostChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not SectionHostView view)
        {
            return;
        }

        if (newValue is ISectionHost host)
        {
            view.CurrentKey = host.CurrentKey;
            view.Current = host.Current;
            if (host is SectionHostViewModel section)
            {
                view.SelectCommand = section.SelectCommand;
            }
        }
    }
}
