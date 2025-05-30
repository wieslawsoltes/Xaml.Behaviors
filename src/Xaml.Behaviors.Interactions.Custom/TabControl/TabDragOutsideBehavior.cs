using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Behavior that detects when a <see cref="TabItem"/> is dragged outside of its parent <see cref="TabControl"/>.
/// </summary>
public class TabDragOutsideBehavior : StyledElementBehavior<TabItem>
{
    private bool _captured;
    private TabControl? _tabControl;

    /// <summary>
    /// Occurs when the associated <see cref="TabItem"/> is dragged outside of its parent <see cref="TabControl"/>.
    /// </summary>
    public event EventHandler<PointerEventArgs>? DragOutside;

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveHandler(InputElement.PointerPressedEvent, OnPointerPressed);
            AssociatedObject.RemoveHandler(InputElement.PointerReleasedEvent, OnPointerReleased);
            AssociatedObject.RemoveHandler(InputElement.PointerMovedEvent, OnPointerMoved);
            AssociatedObject.RemoveHandler(InputElement.PointerCaptureLostEvent, OnPointerCaptureLost);
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed && AssociatedObject?.Parent is TabControl tc)
        {
            _tabControl = tc;
            _captured = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _captured = false;
    }

    private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _captured = false;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_captured || _tabControl is null)
        {
            return;
        }

        var position = e.GetPosition(_tabControl);
        var bounds = _tabControl.Bounds;
        if (position.X < 0 || position.Y < 0 || position.X > bounds.Width || position.Y > bounds.Height)
        {
            _captured = false;
            DragOutside?.Invoke(AssociatedObject, e);
        }
    }
}
