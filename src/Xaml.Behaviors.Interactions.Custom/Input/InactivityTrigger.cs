// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A trigger that fires when the user has been inactive (no mouse/keyboard input) for a specified duration.
/// </summary>
public class InactivityTrigger : Trigger<Control>
{
    private DispatcherTimer? _timer;
    private TopLevel? _topLevel;
    private IDisposable? _pointerMovedDisposable;
    private IDisposable? _keyDownDisposable;

    /// <summary>
    /// Identifies the <seealso cref="Timeout"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TimeSpan> TimeoutProperty =
        AvaloniaProperty.Register<InactivityTrigger, TimeSpan>(nameof(Timeout), TimeSpan.FromSeconds(5));

    /// <summary>
    /// Gets or sets the inactivity timeout duration.
    /// </summary>
    public TimeSpan Timeout
    {
        get => GetValue(TimeoutProperty);
        set => SetValue(TimeoutProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        _topLevel = TopLevel.GetTopLevel(AssociatedObject);
        if (_topLevel != null)
        {
            _timer = new DispatcherTimer
            {
                Interval = Timeout
            };
            _timer.Tick += Timer_Tick;

            // Listen to global events on TopLevel
            _topLevel.AddHandler(InputElement.PointerMovedEvent, OnInput, RoutingStrategies.Tunnel);
            _topLevel.AddHandler(InputElement.KeyDownEvent, OnInput, RoutingStrategies.Tunnel);
            
            _timer.Start();
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Tick -= Timer_Tick;
            _timer = null;
        }

        if (_topLevel != null)
        {
            _topLevel.RemoveHandler(InputElement.PointerMovedEvent, OnInput);
            _topLevel.RemoveHandler(InputElement.KeyDownEvent, OnInput);
            _topLevel = null;
        }
    }

    private void OnInput(object? sender, RoutedEventArgs e)
    {
        ResetTimer();
    }

    private void ResetTimer()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Interval = Timeout; // Update interval in case property changed
            _timer.Start();
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _timer?.Stop();
        Interaction.ExecuteActions(AssociatedObject, Actions, null);
    }
}
