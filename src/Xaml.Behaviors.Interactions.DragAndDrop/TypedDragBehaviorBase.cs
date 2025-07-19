// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior base class that initiates a drag operation for a specific data type.
/// </summary>
public abstract class TypedDragBehaviorBase : StyledElementBehavior<Control>
{
    private Point _dragStartPoint;
    private PointerEventArgs? _triggerEvent;
    private object? _value;
    private bool _lock;

    /// <summary>
    /// Identifies the <see cref="DataFormat"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string> DataFormatProperty =
        AvaloniaProperty.Register<TypedDragBehaviorBase, string>(nameof(DataFormat), "Context");

    /// <summary>
    /// Gets or sets the data format used for drag operations.
    /// </summary>
    public string DataFormat
    {
        get => GetValue(DataFormatProperty);
        set => SetValue(DataFormatProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="DataType"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Type?> DataTypeProperty =
        AvaloniaProperty.Register<TypedDragBehaviorBase, Type?>(nameof(DataType));

    /// <summary>
    /// Gets or sets the data type allowed for dragging.
    /// </summary>
    public Type? DataType
    {
        get => GetValue(DataTypeProperty);
        set => SetValue(DataTypeProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.PointerPressedEvent, AssociatedObject_PointerPressed);
        AssociatedObject?.RemoveHandler(InputElement.PointerReleasedEvent, AssociatedObject_PointerReleased);
        AssociatedObject?.RemoveHandler(InputElement.PointerMovedEvent, AssociatedObject_PointerMoved);
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

    private async Task DoDragDrop(PointerEventArgs triggerEvent, object? value)
    {
        var data = new DataObject();
        data.Set(DataFormat, value!);

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

        await DragDrop.DoDragDrop(triggerEvent, data, effect);
    }

    private void AssociatedObject_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed)
        {
            if (e.Source is Control control 
                && AssociatedObject?.DataContext == control.DataContext
                && DataType is not null
                && DataType.IsInstanceOfType(control.DataContext))
            {
                _dragStartPoint = e.GetPosition(null);
                _triggerEvent = e;
                _value = control.DataContext;
                _lock = true;
            }
        }
    }

    private void AssociatedObject_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased && _triggerEvent is not null)
        {
            _triggerEvent = null;
            _value = null;
            _lock = false;
        }
    }

    private async void AssociatedObject_PointerMoved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed && _triggerEvent is not null)
        {
            var point = e.GetPosition(null);
            var diff = _dragStartPoint - point;
            if (Math.Abs(diff.X) > 3 || Math.Abs(diff.Y) > 3)
            {
                if (_lock)
                {
                    _lock = false;
                }
                else
                {
                    return;
                }

                OnBeforeDragDrop(sender, _triggerEvent, _value);

                await DoDragDrop(_triggerEvent, _value);

                OnAfterDragDrop(sender, _triggerEvent, _value);

                _triggerEvent = null;
                _value = null;
            }
        }
    }
}
