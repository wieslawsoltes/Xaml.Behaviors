// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
using Windows.Foundation;
#else
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
/// Behavior base class that starts a drag operation using the associated context data.
/// </summary>
public abstract partial class ContextDragBehaviorBase : StyledElementBehavior<Control>
{
    private Point _dragStartPoint;
    private PointerPressedEventArgs? _triggerEvent;
    private bool _lock;
    private bool _captured;

    /// <summary>
    /// Gets or sets context data passed to the drag handler.
    /// </summary>
    [StyledProperty]
    public partial object? Context { get; set; }

    /// <summary>
    /// Gets or sets the horizontal distance in pixels required to start a drag.
    /// </summary>
    [StyledProperty(DefaultValue = 3)]
    public partial double HorizontalDragThreshold { get; set; }

    /// <summary>
    /// Gets or sets the vertical distance in pixels required to start a drag.
    /// </summary>
    [StyledProperty(DefaultValue = 3)]
    public partial double VerticalDragThreshold { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, AssociatedObject_KeyDown, RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed);
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased);
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved);
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.PointerCaptureLostEvent, AssociatedObject_CaptureLost);
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.KeyDownEvent, AssociatedObject_KeyDown);
#if WINUI
        StopEscapeTracking();
#endif
    }

    /// <summary>
    /// Called before the drag operation begins.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="context"></param>
    protected abstract void OnBeforeDragDrop(object? sender, PointerEventArgs e, object? context);

    /// <summary>
    /// Called after the drag operation completes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="context"></param>
    protected abstract void OnAfterDragDrop(object? sender, PointerEventArgs e, object? context);

    private async Task DoDragDrop(PointerPressedEventArgs triggerEvent, object? value)
    {
        var data = new DataTransfer();
        var contextKey = DragDropContextStore.Add(value);

        if (contextKey is not null)
        {
            data.Add(DataTransferItem.Create(ContextDropBehaviorBase.ContextDataTransferFormat, contextKey));
        }

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
#if WINUI
        StopEscapeTracking();
#endif
    }

#if WINUI
    // Native WinUI can move the focus to the root of the window when the pointer is pressed, so the Escape key does
    // not reach the associated object: while a press is pending the key is handled at the root.
    private UIElement? _escapeRoot;
    private Microsoft.UI.Xaml.Input.KeyEventHandler? _escapeHandler;

    private void StartEscapeTracking()
    {
        StopEscapeTracking();
        if (AssociatedObject?.XamlRoot?.Content is not { } root)
        {
            return;
        }

        _escapeRoot = root;
        _escapeHandler = (_, e) =>
        {
            if (e.Key == Key.Escape)
            {
                Released();
                _captured = false;
            }
        };
        root.AddHandler(UIElement.KeyDownEvent, _escapeHandler, true);
    }

    private void StopEscapeTracking()
    {
        if (_escapeRoot is not null && _escapeHandler is not null)
        {
            _escapeRoot.RemoveHandler(UIElement.KeyDownEvent, _escapeHandler);
        }

        _escapeRoot = null;
        _escapeHandler = null;
    }
#endif

    private void AssociatedObject_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed && IsEnabled)
        {
            if (AssociatedObject is not null
                && DragSourcePressFilter.BelongsToDragSource(AssociatedObject, e.Source))
            {
                _dragStartPoint = e.GetPosition(null);
                _triggerEvent = e;
                _lock = true;
                _captured = true;
#if WINUI
                StartEscapeTracking();
#endif

                // Drag detection must not consume the initial press. Selection and
                // interactive content still need to observe it before a drag starts.
#if UNO
                // WinUI has no pointer tunnel phase: the handler also receives handled events, after the
                // element's own handlers, so resetting Handled would un-handle a press the element (for
                // example a list item) consumed. The press is left as is.
#else
                e.Handled = false;
#endif
                return;
            }
        }
#if !UNO
        e.Handled = false;
#endif
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
#if WINUI
            StopEscapeTracking();
#endif
        }
    }

    private async void AssociatedObject_PointerMoved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (_captured
            && properties.IsLeftButtonPressed && IsEnabled &&
            _triggerEvent is not null)
        {
            var point = e.GetPosition(null);
            var diffX = _dragStartPoint.X - point.X;
            var diffY = _dragStartPoint.Y - point.Y;
            var horizontalDragThreshold = HorizontalDragThreshold;
            var verticalDragThreshold = VerticalDragThreshold;

            if (Math.Abs(diffX) > horizontalDragThreshold || Math.Abs(diffY) > verticalDragThreshold)
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
                    
                OnBeforeDragDrop(sender, _triggerEvent, context);

                await DoDragDrop(_triggerEvent, context);

                OnAfterDragDrop(sender, _triggerEvent, context);

                _triggerEvent = null;
            }
        }
    }

    private void AssociatedObject_CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        Released();
        _captured = false;
    }

    private void AssociatedObject_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Released();
            _captured = false;
        }
    }
}
