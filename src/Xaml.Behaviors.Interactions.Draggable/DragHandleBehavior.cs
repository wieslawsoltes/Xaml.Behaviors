using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Enables dragging of a control using a dedicated handle element.
/// </summary>
public class DragHandleBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="Handle"/> attached avalonia property.
    /// </summary>
    public static readonly AttachedProperty<Control?> HandleProperty =
        AvaloniaProperty.RegisterAttached<Control, Control?>(
            "Handle",
            typeof(DragHandleBehavior));

    private bool _captured;
    private Point _start;
    private Control? _parent;
    private TranslateTransform? _transform;
    private Control? _handle;

    /// <summary>
    /// Gets the handle associated with the specified control.
    /// </summary>
    public static Control? GetHandle(AvaloniaObject element)
    {
        return element.GetValue(HandleProperty);
    }

    /// <summary>
    /// Sets the handle associated with the specified control.
    /// </summary>
    public static void SetHandle(AvaloniaObject element, Control? value)
    {
        element.SetValue(HandleProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        _handle = GetHandle(AssociatedObject!);
        var target = _handle ?? AssociatedObject;
        if (target is not null)
        {
            target.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Tunnel);
            target.AddHandler(InputElement.PointerReleasedEvent, Released, RoutingStrategies.Tunnel);
            target.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Tunnel);
            target.AddHandler(InputElement.PointerCaptureLostEvent, CaptureLost, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        var target = _handle ?? AssociatedObject;
        if (target is not null)
        {
            target.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
            target.RemoveHandler(InputElement.PointerReleasedEvent, Released);
            target.RemoveHandler(InputElement.PointerMovedEvent, Moved);
            target.RemoveHandler(InputElement.PointerCaptureLostEvent, CaptureLost);
        }
    }

    private void Pressed(object? sender, PointerPressedEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (properties.IsLeftButtonPressed && AssociatedObject?.Parent is Control parent)
        {
            _parent = parent;
            _start = e.GetPosition(_parent);

            if (AssociatedObject.RenderTransform is TranslateTransform tr)
            {
                _transform = tr;
            }
            else
            {
                _transform = new TranslateTransform();
                AssociatedObject.RenderTransform = _transform;
            }

            _captured = true;
        }
    }

    private void Released(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured && e.InitialPressMouseButton == MouseButton.Left)
        {
            EndDrag();
        }
    }

    private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (_captured)
        {
            EndDrag();
        }
    }

    private void Moved(object? sender, PointerEventArgs e)
    {
        var properties = e.GetCurrentPoint(AssociatedObject).Properties;
        if (!_captured || !properties.IsLeftButtonPressed || _parent is null || _transform is null)
        {
            return;
        }

        var position = e.GetPosition(_parent);
        var deltaX = position.X - _start.X;
        var deltaY = position.Y - _start.Y;
        _start = position;

        _transform.X += deltaX;
        _transform.Y += deltaY;
    }

    private void EndDrag()
    {
        _captured = false;
        _parent = null;
        _transform = null;
    }
}
