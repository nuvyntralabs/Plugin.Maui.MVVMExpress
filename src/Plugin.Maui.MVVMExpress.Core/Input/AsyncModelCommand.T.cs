using System.Windows.Input;
using Plugin.Maui.MVVMExpress.ComponentModel;

namespace Plugin.Maui.MVVMExpress.Input;

/// <summary>Async command with a typed parameter and single-flight execution.</summary>
/// <typeparam name="T">Parameter type.</typeparam>
public sealed class AsyncModelCommand<T> : ObservableModel, ICommand
{
    private readonly Func<T?, CancellationToken, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private readonly AsyncCommandOptions _options;
    private CancellationTokenSource? _execution;
    private int _runLock;
    private CommandExecutionState _state = CommandExecutionState.Idle;
    private bool _isRunning;

    /// <summary>Creates an async command.</summary>
    /// <param name="execute">Work to run.</param>
    /// <param name="canExecute">Optional predicate.</param>
    /// <param name="options">Timeout, retry, and concurrency.</param>
    public AsyncModelCommand(
        Func<T?, CancellationToken, Task> execute,
        Func<T?, bool>? canExecute = null,
        AsyncCommandOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        _execute = execute;
        _canExecute = canExecute;
        _options = options ?? new AsyncCommandOptions();
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>Gets a value indicating whether the command is executing.</summary>
    public bool IsRunning
    {
        get => _isRunning;
        private set => SetProperty(ref _isRunning, value);
    }

    /// <summary>Gets the last / current execution state.</summary>
    public CommandExecutionState State
    {
        get => _state;
        private set => SetProperty(ref _state, value);
    }

    /// <summary>Gets a value indicating whether cancellation was requested for the current run.</summary>
    public bool IsCancellationRequested => _execution?.IsCancellationRequested == true;

    /// <inheritdoc />
    public bool CanExecute(object? parameter)
    {
        if (IsRunning && _options.Concurrency != ConcurrencyMode.CancelPrevious)
        {
            return false;
        }

        if (!TryCast(parameter, out var value))
        {
            return false;
        }

        return _canExecute?.Invoke(value) ?? true;
    }

    /// <inheritdoc />
    public async void Execute(object? parameter)
    {
        if (TryCast(parameter, out var value))
        {
            await ExecuteAsync(value).ConfigureAwait(false);
        }
    }

    /// <summary>Executes the command.</summary>
    /// <param name="parameter">Typed argument.</param>
    /// <param name="cancellationToken">Caller token.</param>
    public async Task ExecuteAsync(T? parameter, CancellationToken cancellationToken = default)
    {
        if (_options.Concurrency == ConcurrencyMode.CancelPrevious && IsRunning)
        {
            Cancel();
        }
        else if (IsRunning || !(_canExecute?.Invoke(parameter) ?? true))
        {
            return;
        }

        if (Interlocked.CompareExchange(ref _runLock, 1, 0) != 0)
        {
            if (_options.Concurrency != ConcurrencyMode.CancelPrevious)
            {
                return;
            }

            while (Interlocked.CompareExchange(ref _runLock, 1, 0) != 0)
            {
                await Task.Yield();
            }
        }

        CancellationTokenSource? linked = null;
        try
        {
            linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (_options.Timeout is { } timeout)
            {
                linked.CancelAfter(timeout);
            }

            _execution = linked;
            IsRunning = true;
            State = CommandExecutionState.Running;
            NotifyCanExecuteChanged();
            var attempts = 0;
            while (true)
            {
                try
                {
                    await _execute(parameter, linked.Token).ConfigureAwait(false);
                    State = CommandExecutionState.Completed;
                    break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch when (attempts < _options.RetryCount)
                {
                    attempts++;
                    if (_options.RetryDelay > TimeSpan.Zero)
                    {
                        await Task.Delay(_options.RetryDelay, linked.Token).ConfigureAwait(false);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            State = CommandExecutionState.Cancelled;
        }
        catch
        {
            State = CommandExecutionState.Failed;
            throw;
        }
        finally
        {
            if (linked is not null && ReferenceEquals(_execution, linked))
            {
                _execution = null;
            }

            linked?.Dispose();
            IsRunning = false;
            Interlocked.Exchange(ref _runLock, 0);
            NotifyCanExecuteChanged();
        }
    }

    /// <summary>Requests cancellation of the in-flight execution.</summary>
    public void Cancel()
    {
        try
        {
            _execution?.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // Execution already finished.
        }
    }

    /// <summary>Raises <see cref="CanExecuteChanged"/>.</summary>
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    private static bool TryCast(object? parameter, out T? value)
    {
        if (parameter is null)
        {
            value = default;
            return default(T) is null;
        }

        if (parameter is T typed)
        {
            value = typed;
            return true;
        }

        value = default;
        return false;
    }
}
