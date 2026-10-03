// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using Windows.Foundation;
#else
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Determines if the behavior applies to the associated element or its children.
/// </summary>
public enum FluidMoveScope
{
    /// <summary>
    /// Apply behavior to the element itself.
    /// </summary>
    Self,
    /// <summary>
    /// Apply behavior to the children of the element.
    /// </summary>
    Children
}

/// <summary>
/// Behavior that animates position changes of a control or its children.
/// </summary>
/// <remarks>
/// With <see cref="FluidMoveScope.Children"/> the behavior animates the children of a <see cref="Panel"/>, or the
/// item containers of an <see cref="ItemsControl"/> (the children of its items panel). The children of an items panel
/// are tracked by their item, so an item moves smoothly even when its container is recreated.
/// </remarks>
public partial class FluidMoveBehavior : Behavior<Visual>
{
#if UNO
    private Dictionary<object, Point> _positions = new();
    private Dictionary<object, Point> _currentPositions = new();
#else
    private Dictionary<object, PixelPoint> _positions = new();
    private Dictionary<object, PixelPoint> _currentPositions = new();
#endif

    /// <summary>
    /// Gets or sets how the behavior is applied.
    /// </summary>
    [StyledProperty]
    public partial FluidMoveScope AppliesTo { get; set; }

    /// <summary>
    /// Gets or sets animation duration.
    /// </summary>
    [StyledProperty(DefaultValueExpression = "TimeSpan.FromMilliseconds(300)")]
    public partial TimeSpan Duration { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is Layoutable v)
        {
            v.LayoutUpdated += OnLayoutUpdated;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is Layoutable v)
        {
            v.LayoutUpdated -= OnLayoutUpdated;
        }

        _positions.Clear();
        _currentPositions.Clear();
        base.OnDetaching();
    }

#if UNO
    private void OnLayoutUpdated(object? sender, object e)
#else
    private void OnLayoutUpdated(object? sender, EventArgs e)
#endif
    {
        if (AssociatedObject is not { } root)
        {
            return;
        }

        if (AppliesTo == FluidMoveScope.Self)
        {
            if (root is Control c)
            {
                UpdateControl(c, root);
            }
        }
        else if (GetChildrenPanel(root) is { } panel)
        {
            UpdateChildren(panel);
        }
    }

    private static Panel? GetChildrenPanel(Visual root)
    {
        return root switch
        {
            Panel panel => panel,
            ItemsControl itemsControl => itemsControl.ItemsPanelRoot,
            _ => null
        };
    }

    private void UpdateChildren(Panel panel)
    {
        var children = panel.Children;
        var owner = GetItemsOwner(panel);
        var current = _currentPositions;
        current.Clear();

        for (var i = 0; i < children.Count; i++)
        {
            if (children[i] is not Control child)
            {
                continue;
            }

            var key = GetKey(owner, child, current);
            var position = GetLayoutPosition(child);
            if (_positions.TryGetValue(key, out var previous) && previous != position)
            {
                FluidMoveAnimation.TryRun(child, previous.X - position.X, previous.Y - position.Y, Duration);
            }

            current[key] = position;
        }

        // Children that left the panel are forgotten.
        _currentPositions = _positions;
        _positions = current;
    }

    private static ItemsControl? GetItemsOwner(Panel panel)
    {
        var children = panel.Children;
        for (var i = 0; i < children.Count; i++)
        {
            if (children[i] is Control child)
            {
                return ItemsControl.ItemsControlFromItemContainer(child);
            }
        }

        return null;
    }

#if UNO
    private static object GetKey(ItemsControl? owner, Control child, Dictionary<object, Point> current)
#else
    private static object GetKey(ItemsControl? owner, Control child, Dictionary<object, PixelPoint> current)
#endif
    {
        // An item container is tracked by its item (containers are recreated when items move); duplicate items
        // fall back to their containers.
        var item = owner?.ItemFromContainer(child);
        return item is null || current.ContainsKey(item) ? child : item;
    }

#if UNO
    private static Point GetLayoutPosition(Control child)
    {
        // The layout offset in the panel, without the render transform of a running animation.
        var offset = child.ActualOffset;
        return new Point((int)offset.X, (int)offset.Y);
    }
#else
    private static PixelPoint GetLayoutPosition(Control child)
    {
        // The layout offset in the panel, without the render transform of a running animation.
        var position = child.Bounds.Position;
        return new PixelPoint((int)position.X, (int)position.Y);
    }
#endif

    private void UpdateControl(Control control, Visual root)
    {
#if UNO
        var p = control.TransformToVisual(root).TransformPoint(new Point(0, 0));
        var current = new Point((int)p.X, (int)p.Y);
#else
        var p = control.TranslatePoint(new Point(0, 0), root);
        if (p is null)
        {
            return;
        }

        var current = new PixelPoint((int)p.Value.X, (int)p.Value.Y);
#endif

        if (!_positions.TryGetValue(control, out var previous))
        {
            _positions[control] = current;
            return;
        }

        if (previous == current)
        {
            return;
        }

        var dx = previous.X - current.X;
        var dy = previous.Y - current.Y;

        FluidMoveAnimation.TryRun(control, dx, dy, Duration);

        _positions[control] = current;
    }
}
