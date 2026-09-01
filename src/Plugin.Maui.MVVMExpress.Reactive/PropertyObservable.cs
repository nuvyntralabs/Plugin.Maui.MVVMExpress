using System.ComponentModel;

namespace Plugin.Maui.MVVMExpress.Reactive;

/// <summary>Builds <see cref="IPropertyObservable{T}"/> from INPC properties without System.Reactive.</summary>
public static class PropertyObservable
{
    /// <summary>Observes <paramref name="propertyName"/> on <paramref name="source"/>.</summary>
    public static IPropertyObservable<T> Observe<T>(
        INotifyPropertyChanged source,
        string propertyName,
        Func<T> getter)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        ArgumentNullException.ThrowIfNull(getter);
        return new NotifyPropertyObservable<T>(source, propertyName, getter);
    }

    /// <summary>Projects two sources into one value whenever either changes.</summary>
    public static IPropertyObservable<TResult> CombineLatest<T1, T2, TResult>(
        IPropertyObservable<T1> first,
        IPropertyObservable<T2> second,
        Func<T1, T2, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(selector);
        return new CombineLatestObservable<T1, T2, TResult>(first, second, selector);
    }
}
