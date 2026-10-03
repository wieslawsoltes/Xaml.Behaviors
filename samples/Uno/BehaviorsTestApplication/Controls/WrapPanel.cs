// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace BehaviorsTestApplication.Controls;

/// <summary>
/// Arranges its children in rows (or columns) that wrap, like the Avalonia <c>WrapPanel</c> used by the samples.
/// </summary>
/// <remarks>
/// Native WinUI has no wrap panel (Uno Platform has one): the samples use this panel on both platforms.
/// </remarks>
public partial class WrapPanel : Panel
{
    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(WrapPanel),
        new PropertyMetadata(Orientation.Horizontal, (d, _) => ((WrapPanel)d).InvalidateMeasure()));

    /// <summary>
    /// Gets or sets the direction in which the children are arranged before wrapping.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <inheritdoc />
    protected override Size MeasureOverride(Size availableSize)
    {
        var horizontal = Orientation == Orientation.Horizontal;
        var limit = horizontal ? availableSize.Width : availableSize.Height;
        double line = 0, lineThickness = 0, extent = 0, thickness = 0;
        foreach (var child in Children)
        {
            child.Measure(availableSize);
            var (length, childThickness) = horizontal ? (child.DesiredSize.Width, child.DesiredSize.Height) : (child.DesiredSize.Height, child.DesiredSize.Width);
            if (line > 0 && line + length > limit)
            {
                extent = Math.Max(extent, line);
                thickness += lineThickness;
                line = 0;
                lineThickness = 0;
            }

            line += length;
            lineThickness = Math.Max(lineThickness, childThickness);
        }

        extent = Math.Max(extent, line);
        thickness += lineThickness;
        return horizontal ? new Size(extent, thickness) : new Size(thickness, extent);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        var horizontal = Orientation == Orientation.Horizontal;
        var limit = horizontal ? finalSize.Width : finalSize.Height;
        double line = 0, lineThickness = 0, offset = 0;
        foreach (var child in Children)
        {
            var (length, childThickness) = horizontal ? (child.DesiredSize.Width, child.DesiredSize.Height) : (child.DesiredSize.Height, child.DesiredSize.Width);
            if (line > 0 && line + length > limit)
            {
                offset += lineThickness;
                line = 0;
                lineThickness = 0;
            }

            child.Arrange(horizontal
                ? new Rect(line, offset, length, childThickness)
                : new Rect(offset, line, childThickness, length));
            line += length;
            lineThickness = Math.Max(lineThickness, childThickness);
        }

        return finalSize;
    }
}
