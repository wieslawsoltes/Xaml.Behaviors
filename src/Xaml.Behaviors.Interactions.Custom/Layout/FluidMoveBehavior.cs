// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Linq;
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
public partial class FluidMoveBehavior : Behavior<Visual>
{
#if UNO
    private readonly Dictionary<Control, Point> _positions = new();
#else
    private readonly Dictionary<Control, PixelPoint> _positions = new();
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

        if (AppliesTo == FluidMoveScope.Self && root is Control c)
        {
            UpdateControl(c, root);
        }
        else if (AppliesTo == FluidMoveScope.Children && root is Panel panel)
        {
            foreach (var child in panel.Children.OfType<Control>())
            {
                UpdateControl(child, root);
            }
        }
    }

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
