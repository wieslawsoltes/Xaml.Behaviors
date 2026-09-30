// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Disables the associated control while the command cannot execute.
/// </summary>
/// <remarks>
/// Uno Platform counterpart of the Avalonia binder. The disabled value is applied with the animation value
/// precedence so the local value or binding of <c>IsEnabled</c> is preserved and restored afterwards.
/// </remarks>
internal sealed class CommandCanExecuteIsEnabledBinder : IDisposable
{
    private static readonly DependencyProperty s_blockCountProperty =
        DependencyProperty.RegisterAttached(
            "CommandCanExecuteIsEnabledBlockCount",
            typeof(int),
            typeof(CommandCanExecuteIsEnabledBinder),
            new PropertyMetadata(0));

    private Microsoft.UI.Xaml.Controls.Control? _target;
    private IDisposable? _canExecuteSubscription;
    private bool _isBlocking;

    public void Update(UIElement? target, bool enabled, IObservable<bool> canExecute)
    {
        var control = target as Microsoft.UI.Xaml.Controls.Control;
        if (!enabled || control is null)
        {
            Stop();
            return;
        }

        if (ReferenceEquals(_target, control) && _canExecuteSubscription is not null)
        {
            return;
        }

        Stop();

        _target = control;
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

    private static void AddDisabledOverlay(Microsoft.UI.Xaml.Controls.Control target)
    {
        var count = (int)target.GetValue(s_blockCountProperty) + 1;
        target.SetValue(s_blockCountProperty, count);
        if (count == 1)
        {
            target.SetValue(Microsoft.UI.Xaml.Controls.Control.IsEnabledProperty, false, DependencyPropertyValuePrecedences.Animations);
        }
    }

    private static void RemoveDisabledOverlay(Microsoft.UI.Xaml.Controls.Control target)
    {
        var count = (int)target.GetValue(s_blockCountProperty) - 1;
        if (count > 0)
        {
            target.SetValue(s_blockCountProperty, count);
            return;
        }

        target.ClearValue(s_blockCountProperty);
        target.SetValue(Microsoft.UI.Xaml.Controls.Control.IsEnabledProperty, DependencyProperty.UnsetValue, DependencyPropertyValuePrecedences.Animations);
    }
}
