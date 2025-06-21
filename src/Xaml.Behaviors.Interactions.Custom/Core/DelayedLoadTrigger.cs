// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Invokes its actions after the associated control is attached to the visual tree and a delay elapses.
/// </summary>
public sealed class DelayedLoadTrigger : StyledElementTrigger<Control>
{
    /// <summary>
    /// Identifies the <see cref="Delay"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> DelayProperty =
        AvaloniaProperty.Register<DelayedLoadTrigger, TimeSpan>(nameof(Delay), TimeSpan.FromMilliseconds(500));

    private DispatcherTimer? _timer;

    /// <summary>
    /// Gets or sets the delay before actions are executed.
    /// </summary>
    public TimeSpan Delay
    {
        get => GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _timer = new DispatcherTimer { Interval = Delay };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        DisposeTimer();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        Execute(parameter: null);
        DisposeTimer();
    }

    private void Execute(object? parameter)
    {
        if (!IsEnabled)
        {
            return;
        }

        Interaction.ExecuteActions(AssociatedObject, Actions, parameter);
    }

    private void DisposeTimer()
    {
        if (_timer is not null)
        {
            _timer.Tick -= OnTick;
            _timer.Stop();
            _timer = null;
        }
    }
}
