using CommunityToolkit.Mvvm.Messaging;
using Plugin.Maui.MVVMExpress.Messaging;

namespace Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit;

/// <summary>Adapts CommunityToolkit <see cref="IMessenger"/> to <see cref="IMessageHub"/> without type-forwarding names.</summary>
public sealed class CommunityToolkitMessageHub : IMessageHub
{
    private readonly IMessenger _messenger;

    /// <summary>Creates an adapter over <paramref name="messenger"/>.</summary>
    public CommunityToolkitMessageHub(IMessenger messenger)
    {
        ArgumentNullException.ThrowIfNull(messenger);
        _messenger = messenger;
    }

    /// <inheritdoc />
    /// <remarks>
    /// CommunityToolkit <see cref="IMessenger"/> requires reference-type messages. This adapter wraps every
    /// payload so value types still satisfy <see cref="IMessageHub"/>. The <paramref name="weak"/> flag is
    /// ignored; lifetime follows the injected <see cref="IMessenger"/> (strong vs weak).
    /// </remarks>
    public IDisposable Subscribe<TRecipient, TMessage>(
        TRecipient subscriber,
        Action<TRecipient, TMessage> handler,
        bool weak = true)
        where TRecipient : class
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(handler);
        _ = weak;
        _messenger.Register<Envelope<TMessage>>(subscriber, (_, envelope) => handler(subscriber, envelope.Value));
        return new Subscription(() => _messenger.Unregister<Envelope<TMessage>>(subscriber));
    }

    /// <inheritdoc />
    public void Publish<TMessage>(TMessage message)
    {
        _messenger.Send(new Envelope<TMessage>(message));
    }

    /// <inheritdoc />
    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Publish(message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Unsubscribe(object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        _messenger.UnregisterAll(subscriber);
    }

    private sealed class Envelope<T>
    {
        public Envelope(T value) => Value = value;

        public T Value { get; }
    }

    private sealed class Subscription : IDisposable
    {
        private readonly Action _unsubscribe;
        private bool _disposed;

        public Subscription(Action unsubscribe) => _unsubscribe = unsubscribe;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _unsubscribe();
            _disposed = true;
        }
    }
}
