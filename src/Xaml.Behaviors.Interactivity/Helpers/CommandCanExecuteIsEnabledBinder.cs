// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactivity;

internal sealed class CommandCanExecuteIsEnabledBinder : IDisposable
{
    private static readonly AttachedProperty<CommandCanExecuteIsEnabledOverlayState?> OverlayStateProperty =
        AvaloniaProperty.RegisterAttached<CommandCanExecuteIsEnabledBinder, InputElement, CommandCanExecuteIsEnabledOverlayState?>(
            "CommandCanExecuteIsEnabledOverlayState");

    private InputElement? _target;
    private IDisposable? _canExecuteSubscription;
    private bool _isBlocking;

    public void Update(InputElement? target, bool enabled, IObservable<bool> canExecute)
    {
        if (!enabled || target is null)
        {
            Stop();
            return;
        }

        if (ReferenceEquals(_target, target) && _canExecuteSubscription is not null)
        {
            return;
        }

        Stop();

        _target = target;
        _canExecuteSubscription = canExecute.Subscribe(new AnonymousObserver<bool>(UpdateCanExecute));
    }

    public void Stop()
    {
        _canExecuteSubscription?.Dispose();
        _canExecuteSubscription = null;

        if (_isBlocking && _target is not null)
        {
            RemoveDisabledOverlay(_target);
        }

        _isBlocking = false;
        _target = null;
    }

    public void Dispose()
    {
        Stop();
    }

    private void UpdateCanExecute(bool canExecute)
    {
        if (_target is not { } target)
        {
            return;
        }

        if (canExecute)
        {
            if (_isBlocking)
            {
                RemoveDisabledOverlay(target);
                _isBlocking = false;
            }
        }
        else if (!_isBlocking)
        {
            AddDisabledOverlay(target);
            _isBlocking = true;
        }
    }

    private static void AddDisabledOverlay(InputElement target)
    {
        var state = target.GetValue(OverlayStateProperty);
        if (state is null)
        {
            state = new CommandCanExecuteIsEnabledOverlayState();
            target.SetValue(OverlayStateProperty, state);
        }

        state.BlockCount++;
        if (state.BlockCount == 1)
        {
            state.DisabledOverlay = target.SetValue(InputElement.IsEnabledProperty, false, BindingPriority.Animation);
        }
    }

    private static void RemoveDisabledOverlay(InputElement target)
    {
        var state = target.GetValue(OverlayStateProperty);
        if (state is null)
        {
            return;
        }

        state.BlockCount--;
        if (state.BlockCount > 0)
        {
            return;
        }

        state.DisabledOverlay?.Dispose();
        target.ClearValue(OverlayStateProperty);
    }

    private sealed class CommandCanExecuteIsEnabledOverlayState
    {
        public int BlockCount { get; set; }

        public IDisposable? DisabledOverlay { get; set; }
    }
}
