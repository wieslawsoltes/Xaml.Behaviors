// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Events;
#else
namespace Avalonia.Xaml.Interactions.Events;
#endif

/// <summary>
/// Trigger that listens for the <see cref="InputElement.PointerCaptureLostEvent"/>.
/// </summary>
public class PointerCaptureLostEventTrigger : InteractiveTriggerBase
{
    private System.IDisposable? _subscription;

    static PointerCaptureLostEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<PointerCaptureLostEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Direct));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _subscription?.Dispose();
        _subscription = AssociatedObject?.AddDisposableRoutedEventHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        Execute(e);
    }
}
