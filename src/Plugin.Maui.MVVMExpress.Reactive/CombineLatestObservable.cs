namespace Plugin.Maui.MVVMExpress.Reactive;

internal sealed class CombineLatestObservable<T1, T2, TResult> : IPropertyObservable<TResult>
{
    private readonly IPropertyObservable<T1> _first;
    private readonly IPropertyObservable<T2> _second;
    private readonly Func<T1, T2, TResult> _selector;
    private readonly List<Action<TResult>> _observers = [];
    private readonly IDisposable _firstSub;
    private readonly IDisposable _secondSub;
    private T1 _left;
    private T2 _right;
    private bool _disposed;

    public CombineLatestObservable(
        IPropertyObservable<T1> first,
        IPropertyObservable<T2> second,
        Func<T1, T2, TResult> selector)
    {
        _first = first;
        _second = second;
        _selector = selector;
        _left = first.Value;
        _right = second.Value;
        _firstSub = first.Subscribe(value =>
        {
            _left = value;
            Publish();
        });
        _secondSub = second.Subscribe(value =>
        {
            _right = value;
            Publish();
        });
    }

    public TResult Value => _selector(_left, _right);

    public IDisposable Subscribe(Action<TResult> observer)
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

        _firstSub.Dispose();
        _secondSub.Dispose();
        _first.Dispose();
        _second.Dispose();
        _observers.Clear();
        _disposed = true;
    }

    private void Publish()
    {
        var value = Value;
        foreach (var observer in _observers.ToArray())
        {
            observer(value);
        }
    }

    private void Unsubscribe(Action<TResult> observer) => _observers.Remove(observer);

    private sealed class Subscription : IDisposable
    {
        private CombineLatestObservable<T1, T2, TResult>? _owner;
        private readonly Action<TResult> _observer;

        public Subscription(CombineLatestObservable<T1, T2, TResult> owner, Action<TResult> observer)
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
