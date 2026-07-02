// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Windows.Input;

namespace Avalonia.Xaml.Interactivity;

/// <summary>
/// Observes an <see cref="ICommand"/> and reports whether it can execute with a specified parameter.
/// </summary>
/// <remarks>
/// Use this helper from custom command-backed behaviors to expose a bindable command state, such as an
/// Avalonia direct property. Call <see cref="Start(ICommand?, object?)"/> when the behavior is attached,
/// <see cref="Update(ICommand?, object?)"/> when the command or command parameter changes, and <see cref="Stop"/>
/// when the behavior is detached.
/// The command event subscription uses a weak reference to this observer so a long-lived command does not
/// keep the observer or owning behavior alive if cleanup is missed.
/// </remarks>
public sealed class CommandCanExecuteObserver : IDisposable
{
    private readonly Action<bool> _setCanExecute;
    private readonly WeakCanExecuteChangedHandler _weakCanExecuteChangedHandler;
    private readonly EventHandler _canExecuteChangedHandler;
    private ICommand? _command;
    private object? _parameter;
    private bool _isParameterKnown = true;
    private bool _isObserving;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandCanExecuteObserver"/> class.
    /// </summary>
    /// <param name="setCanExecute">The callback that receives the current command execution state.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="setCanExecute"/> is <c>null</c>.</exception>
    public CommandCanExecuteObserver(Action<bool> setCanExecute)
    {
        ArgumentNullException.ThrowIfNull(setCanExecute);

        _setCanExecute = setCanExecute;
        _weakCanExecuteChangedHandler = new WeakCanExecuteChangedHandler(this);
        _canExecuteChangedHandler = _weakCanExecuteChangedHandler.OnCanExecuteChanged;
    }

    /// <summary>
    /// Starts observing the specified command and reports its initial execution state.
    /// </summary>
    /// <param name="command">The command to observe, or <c>null</c> to report that execution is allowed.</param>
    /// <param name="parameter">The command parameter supplied to <see cref="ICommand.CanExecute"/>.</param>
    public void Start(ICommand? command, object? parameter)
    {
        Start(command, parameter, true);
    }

    /// <summary>
    /// Starts observing the specified command and reports its initial execution state.
    /// </summary>
    /// <param name="command">The command to observe, or <c>null</c> to report that execution is allowed.</param>
    /// <param name="parameter">The command parameter supplied to <see cref="ICommand.CanExecute"/>.</param>
    /// <param name="isParameterKnown">
    /// <c>true</c> when <paramref name="parameter"/> is the actual parameter to evaluate; <c>false</c> when the
    /// parameter will only be available at execution time.
    /// </param>
    /// <remarks>
    /// When <paramref name="isParameterKnown"/> is <c>false</c>, the observer reports <c>true</c> without calling
    /// <see cref="ICommand.CanExecute"/>. This avoids probing commands with placeholder values such as <c>null</c>
    /// when event arguments or picker results will provide the real parameter later.
    /// </remarks>
    public void Start(ICommand? command, object? parameter, bool isParameterKnown)
    {
        if (_isObserving)
        {
            Update(command, parameter, isParameterKnown);
            return;
        }

        _isObserving = true;
        _command = command;
        _parameter = parameter;
        _isParameterKnown = isParameterKnown;

        if (command is not null)
        {
            _weakCanExecuteChangedHandler.SetCommand(command);
            command.CanExecuteChanged += _canExecuteChangedHandler;
        }

        UpdateCanExecute();
    }

    /// <summary>
    /// Updates the observed command and command parameter.
    /// </summary>
    /// <param name="command">The command to observe, or <c>null</c> to report that execution is allowed.</param>
    /// <param name="parameter">The command parameter supplied to <see cref="ICommand.CanExecute"/>.</param>
    /// <remarks>
    /// This method has no effect until <see cref="Start(ICommand?, object?)"/> has been called.
    /// </remarks>
    public void Update(ICommand? command, object? parameter)
    {
        Update(command, parameter, true);
    }

    /// <summary>
    /// Updates the observed command and command parameter.
    /// </summary>
    /// <param name="command">The command to observe, or <c>null</c> to report that execution is allowed.</param>
    /// <param name="parameter">The command parameter supplied to <see cref="ICommand.CanExecute"/>.</param>
    /// <param name="isParameterKnown">
    /// <c>true</c> when <paramref name="parameter"/> is the actual parameter to evaluate; <c>false</c> when the
    /// parameter will only be available at execution time.
    /// </param>
    /// <remarks>
    /// This method has no effect until <see cref="Start(ICommand?, object?)"/> has been called.
    /// </remarks>
    public void Update(ICommand? command, object? parameter, bool isParameterKnown)
    {
        if (!_isObserving)
        {
            return;
        }

        if (!ReferenceEquals(_command, command))
        {
            if (_command is not null)
            {
                _command.CanExecuteChanged -= _canExecuteChangedHandler;
            }

            _command = command;

            if (command is not null)
            {
                _weakCanExecuteChangedHandler.SetCommand(command);
                command.CanExecuteChanged += _canExecuteChangedHandler;
            }
            else
            {
                _weakCanExecuteChangedHandler.SetCommand(null);
            }
        }

        _parameter = parameter;
        _isParameterKnown = isParameterKnown;
        UpdateCanExecute();
    }

    /// <summary>
    /// Stops observing the current command and clears the stored command parameter.
    /// </summary>
    public void Stop()
    {
        if (!_isObserving)
        {
            return;
        }

        if (_command is not null)
        {
            _command.CanExecuteChanged -= _canExecuteChangedHandler;
        }

        _command = null;
        _parameter = null;
        _isParameterKnown = true;
        _isObserving = false;
        _weakCanExecuteChangedHandler.SetCommand(null);
    }

    /// <summary>
    /// Stops observing the current command.
    /// </summary>
    public void Dispose()
    {
        Stop();
    }

    private void UpdateCanExecute()
    {
        if (_command is null || !_isParameterKnown)
        {
            _setCanExecute(true);
            return;
        }

        _setCanExecute(_command.CanExecute(_parameter));
    }

    private sealed class WeakCanExecuteChangedHandler
    {
        private readonly WeakReference<CommandCanExecuteObserver> _observer;
        private WeakReference<ICommand>? _command;

        public WeakCanExecuteChangedHandler(CommandCanExecuteObserver observer)
        {
            _observer = new WeakReference<CommandCanExecuteObserver>(observer);
        }

        public void SetCommand(ICommand? command)
        {
            _command = command is null ? null : new WeakReference<ICommand>(command);
        }

        public void OnCanExecuteChanged(object? sender, EventArgs e)
        {
            if (_observer.TryGetTarget(out var observer))
            {
                observer.UpdateCanExecute();
                return;
            }

            ICommand? command = sender as ICommand;
            if (command is null)
            {
                if (_command is null || !_command.TryGetTarget(out command))
                {
                    return;
                }
            }

            command.CanExecuteChanged -= OnCanExecuteChanged;
            _command = null;
        }
    }
}
