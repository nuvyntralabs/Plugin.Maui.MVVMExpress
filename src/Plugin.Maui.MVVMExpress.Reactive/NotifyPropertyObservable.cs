using System.ComponentModel;

namespace Plugin.Maui.MVVMExpress.Reactive;

internal sealed class NotifyPropertyObservable<T> : IPropertyObservable<T>
{
    private readonly INotifyPropertyChanged _source;
    private readonly string _propertyName;
    private readonly Func<T> _getter;
    private readonly List<Action<T>> _observers = [];
    private bool _disposed;

    public NotifyPropertyObservable(INotifyPropertyChanged source, string propertyName, Func<T> getter)
    {
        _source = source;
        _propertyName = propertyName;
        _getter = getter;
        _source.PropertyChanged += OnChanged;
    }

    public T Value => _getter();

    public IDisposable Subscribe(Action<T> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ObjectDisposedException.ThrowIf(_disposed, this);
        _observers.Add(observer);
        observer(Value);
        return new Subscription(this, observer);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _source.PropertyChanged -= OnChanged;
        _observers.Clear();
        _disposed = true;
    }

    private void OnChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not null && e.PropertyName != _propertyName)
        {
            return;
        }

        var value = Value;
        foreach (var observer in _observers.ToArray())
        {
            observer(value);
        }
    }

    private void Unsubscribe(Action<T> observer) => _observers.Remove(observer);

    private sealed class Subscription : IDisposable
    {
        private NotifyPropertyObservable<T>? _owner;
        private readonly Action<T> _observer;

        public Subscription(NotifyPropertyObservable<T> owner, Action<T> observer)
        {
            _owner = owner;
            _observer = observer;
        }

        public void Dispose()
        {
            var owner = Interlocked.Exchange(ref _owner, null);
            owner?.Unsubscribe(_observer);
        }
    }
}
