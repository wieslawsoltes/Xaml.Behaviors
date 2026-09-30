// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Behavior that starts drag and drop with information about drag direction.
/// </summary>
public sealed partial class ContextDragWithDirectionBehavior : StyledElementBehavior<Control>
{
    private Point _dragStartPoint;
    private PointerPressedEventArgs? _triggerEvent;
    private bool _lock;
    private bool _captured;
    private static readonly DataFormat<string> DirectionDataTransferFormat =
        DataFormat.CreateStringApplicationFormat("Avalonia.Xaml.Interactions.DragAndDrop.Direction");

    /// <summary>
    /// Gets or sets the context used for drag operations.
    /// </summary>
    [StyledProperty]
    public partial object? Context { get; set; }

    /// <summary>
    /// Gets or sets the drag handler to notify.
    /// </summary>
    [StyledProperty]
    public partial IDragHandler? Handler { get; set; }

    /// <summary>
    /// Gets or sets the horizontal drag threshold.
    /// </summary>
    [StyledProperty(DefaultValue = 3)]
    public partial double HorizontalDragThreshold { get; set; }

    /// <summary>
    /// Gets or sets the vertical drag threshold.
    /// </summary>
    [StyledProperty(DefaultValue = 3)]
    public partial double VerticalDragThreshold { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost,
            RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed);
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased);
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved);
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost);
    }

    private async Task DoDragDrop(PointerPressedEventArgs triggerEvent, object? value, string direction)
    {
        var data = new DataTransfer();
        var contextKey = DragDropContextStore.Add(value);

        if (contextKey is not null)
        {
            data.Add(DataTransferItem.Create(ContextDropBehaviorBase.ContextDataTransferFormat, contextKey));
        }

        data.Add(DataTransferItem.Create(DirectionDataTransferFormat, direction));

        var effect = DragDropEffects.None;

        if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            effect |= DragDropEffects.Link;
        }
        else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            effect |= DragDropEffects.Move;
        }
        else if (triggerEvent.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            effect |= DragDropEffects.Copy;
        }
        else
        {
            effect |= DragDropEffects.Move;
        }

        try
        {
            await DragDrop.DoDragDropAsync(triggerEvent, data, effect);
        }
        finally
        {
            DragDropContextStore.Remove(contextKey);
        }
    }

    private void Released()
    {
        _triggerEvent = null;
        _lock = false;
    }

    private void AssociatedObject_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed)
        {
            if (e.Source is Control control && AssociatedObject?.DataContext == control.DataContext)
            {
                _dragStartPoint = e.GetPosition(null);
                _triggerEvent = e;
                _lock = true;
                _captured = true;
            }
        }
    }

    private void AssociatedObject_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured)
        {
            if (e.InitialPressMouseButton == MouseButton.Left && _triggerEvent is not null)
            {
                Released();
            }

            _captured = false;
        }
    }

    private async void AssociatedObject_PointerMoved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (_captured && properties.IsLeftButtonPressed && _triggerEvent is not null)
        {
            var point = e.GetPosition(null);
            var diff = _dragStartPoint - point;
            var horizontal = HorizontalDragThreshold;
            var vertical = VerticalDragThreshold;

            if (Math.Abs(diff.X) > horizontal || Math.Abs(diff.Y) > vertical)
            {
                if (_lock)
                {
                    _lock = false;
                }
                else
                {
                    return;
                }

                var context = Context ?? AssociatedObject?.DataContext;

                Handler?.BeforeDragDrop(sender, _triggerEvent, context);

                await DoDragDrop(_triggerEvent, context, diff.Y > 0 ? "up" : "down");

                Handler?.AfterDragDrop(sender, _triggerEvent, context);

                _triggerEvent = null;
            }
        }
    }

    private void AssociatedObject_CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        Released();
        _captured = false;
    }
}
