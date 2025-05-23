using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Enables dragging of a control with the mouse using a <see cref="TranslateTransform"/>.
/// </summary>
public class MouseDragElementBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="ConstrainToParentBounds"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> ConstrainToParentBoundsProperty =
        AvaloniaProperty.Register<MouseDragElementBehavior, bool>(nameof(ConstrainToParentBounds));

    private bool _captured;
    private Point _start;
    private Control? _parent;
    private TranslateTransform? _transform;

    /// <summary>
    /// Gets or sets whether dragging should be constrained to the bounds of the parent control.
    /// </summary>
    public bool ConstrainToParentBounds
    {
        get => GetValue(ConstrainToParentBoundsProperty);
        set => SetValue(ConstrainToParentBoundsProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, Released, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, CaptureLost, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, Released);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, Moved);
            AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, CaptureLost);
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

        var newX = _transform.X + deltaX;
        var newY = _transform.Y + deltaY;

        if (ConstrainToParentBounds && AssociatedObject is Control element)
        {
            var parentBounds = _parent.Bounds;
            var elementBounds = element.Bounds;

            var minX = -elementBounds.X;
            var minY = -elementBounds.Y;
            var maxX = parentBounds.Width - elementBounds.Width - elementBounds.X;
            var maxY = parentBounds.Height - elementBounds.Height - elementBounds.Y;

            newX = Math.Min(Math.Max(newX, minX), maxX);
            newY = Math.Min(Math.Max(newY, minY), maxY);
        }

        _transform.X = newX;
        _transform.Y = newY;
    }

    private void EndDrag()
    {
        _captured = false;
        _parent = null;
        _transform = null;
    }
}
