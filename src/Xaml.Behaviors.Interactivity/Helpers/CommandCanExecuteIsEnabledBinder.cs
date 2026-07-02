// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Input;

namespace Avalonia.Xaml.Interactivity;

internal sealed class CommandCanExecuteIsEnabledBinder : IDisposable
{
    private InputElement? _target;
    private IDisposable? _subscription;

    public void Update(InputElement? target, bool enabled, IObservable<bool> canExecute)
    {
        if (!enabled || target is null)
        {
            Stop();
            return;
        }

        if (ReferenceEquals(_target, target) && _subscription is not null)
        {
            return;
        }

        Stop();

        _target = target;
        _subscription = target.Bind(InputElement.IsEnabledProperty, canExecute);
    }

    public void Stop()
    {
        _subscription?.Dispose();
        _subscription = null;
        _target = null;
    }

    public void Dispose()
    {
        Stop();
    }
}
