// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Draggable;

/// <summary>
/// Automatically scrolls the associated <see cref="ScrollViewer"/> when the pointer is dragged near its edges.
/// </summary>
public class AutoScrollDuringDragBehavior : StyledElementBehavior<ScrollViewer>
{
    /// <summary>
    /// Identifies the <see cref="EdgeDistance"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> EdgeDistanceProperty =
        AvaloniaProperty.Register<AutoScrollDuringDragBehavior, double>(nameof(EdgeDistance), 20);

    /// <summary>
    /// Identifies the <see cref="ScrollDelta"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> ScrollDeltaProperty =
        AvaloniaProperty.Register<AutoScrollDuringDragBehavior, double>(nameof(ScrollDelta), 10);

    private bool _dragging;

    /// <summary>
    /// Gets or sets the distance from the edge that triggers scrolling.
    /// </summary>
    public double EdgeDistance
    {
        get => GetValue(EdgeDistanceProperty);
        set => SetValue(EdgeDistanceProperty, value);
    }

    /// <summary>
    /// Gets or sets the amount scrolled when triggered.
    /// </summary>
    public double ScrollDelta
    {
        get => GetValue(ScrollDeltaProperty);
        set => SetValue(ScrollDeltaProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

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
        base.OnDetachedFromVisualTree();

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
        if (e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed && IsEnabled)
        {
            _dragging = true;
        }
    }

    private void Released(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Left)
        {
            _dragging = false;
        }
    }

    private void CaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _dragging = false;
    }

    private void Moved(object? sender, PointerEventArgs e)
    {
        if (!_dragging || AssociatedObject is null || !IsEnabled)
        {
            return;
        }

        var pos = e.GetPosition(AssociatedObject);
        var bounds = AssociatedObject.Bounds;
        var offset = AssociatedObject.Offset;
        var extent = AssociatedObject.Extent;
        var delta = ScrollDelta;
        var threshold = EdgeDistance;

        double newX = offset.X;
        double newY = offset.Y;

        if (pos.X < threshold)
        {
            newX = Math.Max(newX - delta, 0);
        }
        else if (pos.X > bounds.Width - threshold)
        {
            newX = Math.Min(newX + delta, Math.Max(extent.Width - bounds.Width, 0));
        }

        if (pos.Y < threshold)
        {
            newY = Math.Max(newY - delta, 0);
        }
        else if (pos.Y > bounds.Height - threshold)
        {
            newY = Math.Min(newY + delta, Math.Max(extent.Height - bounds.Height, 0));
        }

        if (Math.Abs(newX - offset.X) > double.Epsilon || Math.Abs(newY - offset.Y) > double.Epsilon)
        {
            AssociatedObject.Offset = new Vector(newX, newY);
        }
    }
}
