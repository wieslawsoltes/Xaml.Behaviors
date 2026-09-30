// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A trigger that fires when the user has been inactive (no mouse/keyboard input) for a specified duration.
/// </summary>
public partial class InactivityTrigger : Trigger<Control>
{
    private DispatcherTimer? _timer;
#if UNO
    private UIElement? _topLevel;
#else
    private TopLevel? _topLevel;
#endif

    /// <summary>
    /// Gets or sets the inactivity timeout duration.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromSeconds(5)")]
    public partial TimeSpan Timeout { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
#if UNO
        // WinUI: the root element of the XAML island hosting the associated object.
        _topLevel = AssociatedObject?.XamlRoot?.Content;
#else
        _topLevel = TopLevel.GetTopLevel(AssociatedObject);
#endif
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
            _topLevel.RemoveRoutedEventHandler(InputElement.PointerMovedEvent, OnInput);
            _topLevel.RemoveRoutedEventHandler(InputElement.KeyDownEvent, OnInput);
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

    private void Timer_Tick(object? sender, object e)
    {
        _timer?.Stop();
        Interaction.ExecuteActions(AssociatedObject, Actions, null);
    }
}
