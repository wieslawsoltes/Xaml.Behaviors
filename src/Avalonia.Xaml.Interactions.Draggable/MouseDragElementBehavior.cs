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
    /// Identifies the <see cref="X"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> XProperty =
        AvaloniaProperty.Register<MouseDragElementBehavior, double>(nameof(X), double.NaN);

    /// <summary>
    /// Identifies the <see cref="Y"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> YProperty =
        AvaloniaProperty.Register<MouseDragElementBehavior, double>(nameof(Y), double.NaN);

    /// <summary>
    /// Identifies the <see cref="ConstrainToParentBounds"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<bool> ConstrainToParentBoundsProperty =
        AvaloniaProperty.Register<MouseDragElementBehavior, bool>(nameof(ConstrainToParentBounds));

    private bool _captured;
    private Point _start;
    private Control? _parent;
    private TranslateTransform? _transform;
    private bool _settingPosition;

    /// <summary>
    /// Occurs when a drag gesture is initiated.
    /// </summary>
    public event EventHandler<PointerEventArgs>? DragBegun;

    /// <summary>
    /// Occurs when a drag gesture update is processed.
    /// </summary>
    public event EventHandler<PointerEventArgs>? Dragging;

    /// <summary>
    /// Occurs when a drag gesture is finished.
    /// </summary>
    public event EventHandler<PointerEventArgs>? DragFinished;

    /// <summary>
    /// Gets or sets whether dragging should be constrained to the bounds of the parent control.
    /// </summary>
    public bool ConstrainToParentBounds
    {
        get => GetValue(ConstrainToParentBoundsProperty);
        set => SetValue(ConstrainToParentBoundsProperty, value);
    }

    /// <summary>
    /// Gets or sets the X position of the dragged element. This is an avalonia property.
    /// </summary>
    public double X
    {
        get => GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    /// <summary>
    /// Gets or sets the Y position of the dragged element. This is an avalonia property.
    /// </summary>
    public double Y
    {
        get => GetValue(YProperty);
        set => SetValue(YProperty, value);
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

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == XProperty || change.Property == YProperty)
        {
            UpdatePosition(new Point(GetValue(XProperty), GetValue(YProperty)));
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

            DragBegun?.Invoke(this, e);
        }
    }

    private void Released(object? sender, PointerReleasedEventArgs e)
    {
        if (_captured && e.InitialPressMouseButton == MouseButton.Left)
        {
            EndDrag();
            DragFinished?.Invoke(this, e);
        }
    }

    private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (_captured)
        {
            EndDrag();
            DragFinished?.Invoke(this, e);
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

        ApplyTranslation(deltaX, deltaY);

        Dragging?.Invoke(this, e);
    }

    private void UpdatePosition(Point point)
    {
        if (_settingPosition || AssociatedObject is null)
        {
            return;
        }

        if (_transform is null)
        {
            if (AssociatedObject.RenderTransform is TranslateTransform tr)
            {
                _transform = tr;
            }
            else
            {
                _transform = new TranslateTransform();
                AssociatedObject.RenderTransform = _transform;
            }
        }

        var current = new Point(_transform.X, _transform.Y);
        var delta = point - current;

        ApplyTranslation(delta.X, delta.Y);
    }

    private void ApplyTranslation(double x, double y)
    {
        if (_transform is null)
        {
            return;
        }

        var newX = _transform.X + x;
        var newY = _transform.Y + y;

        if (ConstrainToParentBounds && AssociatedObject is Control element && _parent is not null)
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

        _settingPosition = true;
        X = newX;
        Y = newY;
        _settingPosition = false;
    }

    private void EndDrag()
    {
        _captured = false;
        _parent = null;
        _transform = null;
    }
}
