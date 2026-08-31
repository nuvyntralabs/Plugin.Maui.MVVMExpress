using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plugin.Maui.MVVMExpress.ComponentModel;

/// <summary>
/// Bindable model with property-change notification. Event args are cached by property name.
/// </summary>
public abstract class ObservableModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Assigns <paramref name="value"/> and raises changing/changed when the value actually differs.
    /// </summary>
    /// <returns><see langword="true"/> if the value changed.</returns>
    protected bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null,
        IEqualityComparer<T>? comparer = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        comparer ??= EqualityComparer<T>.Default;
        if (comparer.Equals(field, value))
        {
            return false;
        }

        NotifyChanging(propertyName);
        field = value;
        Notify(propertyName);
        return true;
    }

    /// <summary>
    /// Assigns <paramref name="value"/> and invokes the changing/changed callbacks when the value differs.
    /// </summary>
    protected bool SetProperty<T>(
        ref T field,
        T value,
        Action<T> onChanging,
        Action<T> onChanged,
        [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(onChanging);
        ArgumentNullException.ThrowIfNull(onChanged);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        var comparer = EqualityComparer<T>.Default;
        if (comparer.Equals(field, value))
        {
            return false;
        }

        NotifyChanging(propertyName);
        onChanging(value);
        field = value;
        onChanged(value);
        Notify(propertyName);
        return true;
    }

    /// <summary>Raises <see cref="PropertyChanged"/> for <paramref name="propertyName"/>.</summary>
    protected void Notify([CallerMemberName] string? propertyName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        PropertyChanged?.Invoke(this, PropertyEventArgsCache.ForChanged(propertyName));
    }

    /// <summary>Raises <see cref="PropertyChanging"/> for <paramref name="propertyName"/>.</summary>
    protected void NotifyChanging([CallerMemberName] string? propertyName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        PropertyChanging?.Invoke(this, PropertyEventArgsCache.ForChanging(propertyName));
    }

    /// <summary>Raises <see cref="PropertyChanged"/> for each name in <paramref name="dependents"/>.</summary>
    protected void NotifyDependsOn(string sourceProperty, params string[] dependents)
    {
        ArgumentException.ThrowIfNullOrEmpty(sourceProperty);
        ArgumentNullException.ThrowIfNull(dependents);
        foreach (var name in dependents)
        {
            Notify(name);
        }
    }
}
