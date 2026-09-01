namespace Plugin.Maui.MVVMExpress.Reactive;

/// <summary>A current value plus change notifications. Does not require System.Reactive.</summary>
/// <typeparam name="T">Value type.</typeparam>
public interface IPropertyObservable<out T> : IDisposable
{
    /// <summary>Latest value.</summary>
    T Value { get; }

    /// <summary>Invokes <paramref name="observer"/> immediately and on each change.</summary>
    IDisposable Subscribe(Action<T> observer);
}
