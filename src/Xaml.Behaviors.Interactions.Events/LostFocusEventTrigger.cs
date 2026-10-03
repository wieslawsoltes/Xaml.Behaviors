// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Events;
#else
namespace Avalonia.Xaml.Interactions.Events;
#endif

/// <summary>
/// Trigger that listens for the <see cref="InputElement.LostFocusEvent"/>.
/// </summary>
public class LostFocusEventTrigger : InteractiveTriggerBase
{
    private System.IDisposable? _subscription;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _subscription?.Dispose();
        _subscription = AssociatedObject?.AddDisposableRoutedEventHandler(InputElement.LostFocusEvent, OnLostFocus, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void OnLostFocus(object? sender, FocusChangedEventArgs e)
    {
        Execute(e);
    }
}
