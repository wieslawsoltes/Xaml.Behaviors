// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Xaml.Interactivity;

namespace BehaviorsTestApplication.Behaviors;

/// <summary>
/// Places the item container of the associated item template root in a <see cref="Canvas"/> or <see cref="Grid"/>
/// items panel.
/// </summary>
/// <remarks>
/// Avalonia binds <c>Canvas.Left</c>/<c>Grid.Column</c> of the item containers with a container style
/// (<c>ItemsControl &gt; ContentPresenter</c>). WinUI style setters cannot bind, so the item template applies the
/// values of its item to its container (the element hosted by the items panel) when it is loaded. Unset values
/// (<see cref="double.NaN"/> or a negative number) are not applied.
/// </remarks>
public class ItemContainerPlacementBehavior : StyledElementBehavior<FrameworkElement>
{
    /// <summary>
    /// Identifies the <see cref="CanvasLeft"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanvasLeftProperty =
        DependencyProperty.Register(nameof(CanvasLeft), typeof(double), typeof(ItemContainerPlacementBehavior), new PropertyMetadata(double.NaN));

    /// <summary>
    /// Identifies the <see cref="CanvasTop"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CanvasTopProperty =
        DependencyProperty.Register(nameof(CanvasTop), typeof(double), typeof(ItemContainerPlacementBehavior), new PropertyMetadata(double.NaN));

    /// <summary>
    /// Identifies the <see cref="GridColumn"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridColumnProperty =
        DependencyProperty.Register(nameof(GridColumn), typeof(int), typeof(ItemContainerPlacementBehavior), new PropertyMetadata(-1));

    /// <summary>
    /// Identifies the <see cref="GridRow"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridRowProperty =
        DependencyProperty.Register(nameof(GridRow), typeof(int), typeof(ItemContainerPlacementBehavior), new PropertyMetadata(-1));

    /// <summary>
    /// Identifies the <see cref="GridColumnSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridColumnSpanProperty =
        DependencyProperty.Register(nameof(GridColumnSpan), typeof(int), typeof(ItemContainerPlacementBehavior), new PropertyMetadata(-1));

    /// <summary>
    /// Identifies the <see cref="GridRowSpan"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty GridRowSpanProperty =
        DependencyProperty.Register(nameof(GridRowSpan), typeof(int), typeof(ItemContainerPlacementBehavior), new PropertyMetadata(-1));

    /// <summary>
    /// Gets or sets the <see cref="Canvas.LeftProperty"/> of the item container.
    /// </summary>
    public double CanvasLeft
    {
        get => (double)GetValue(CanvasLeftProperty);
        set => SetValue(CanvasLeftProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Canvas.TopProperty"/> of the item container.
    /// </summary>
    public double CanvasTop
    {
        get => (double)GetValue(CanvasTopProperty);
        set => SetValue(CanvasTopProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Grid.ColumnProperty"/> of the item container.
    /// </summary>
    public int GridColumn
    {
        get => (int)GetValue(GridColumnProperty);
        set => SetValue(GridColumnProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Grid.RowProperty"/> of the item container.
    /// </summary>
    public int GridRow
    {
        get => (int)GetValue(GridRowProperty);
        set => SetValue(GridRowProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Grid.ColumnSpanProperty"/> of the item container.
    /// </summary>
    public int GridColumnSpan
    {
        get => (int)GetValue(GridColumnSpanProperty);
        set => SetValue(GridColumnSpanProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Grid.RowSpanProperty"/> of the item container.
    /// </summary>
    public int GridRowSpan
    {
        get => (int)GetValue(GridRowSpanProperty);
        set => SetValue(GridRowSpanProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();

        if (FindContainer(AssociatedObject) is not { } container)
        {
            return;
        }

        if (!double.IsNaN(CanvasLeft))
        {
            Canvas.SetLeft(container, CanvasLeft);
        }

        if (!double.IsNaN(CanvasTop))
        {
            Canvas.SetTop(container, CanvasTop);
        }

        if (GridColumn >= 0)
        {
            Grid.SetColumn(container, GridColumn);
        }

        if (GridRow >= 0)
        {
            Grid.SetRow(container, GridRow);
        }

        if (GridColumnSpan > 0)
        {
            Grid.SetColumnSpan(container, GridColumnSpan);
        }

        if (GridRowSpan > 0)
        {
            Grid.SetRowSpan(container, GridRowSpan);
        }
    }

    // The item container is the ancestor (or the element itself) hosted by the items panel.
    private static FrameworkElement? FindContainer(FrameworkElement? element)
    {
        DependencyObject? current = element;
        while (current is FrameworkElement candidate)
        {
            if (VisualTreeHelper.GetParent(candidate) is Panel { IsItemsHost: true })
            {
                return candidate;
            }

            current = VisualTreeHelper.GetParent(candidate);
        }

        return null;
    }
}
