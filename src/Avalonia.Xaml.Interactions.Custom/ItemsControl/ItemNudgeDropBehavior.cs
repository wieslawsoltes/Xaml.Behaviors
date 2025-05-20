using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media.Transformation;
using Avalonia.Xaml.Interactivity;

namespace SW.Core.Views.Behaviors;

public class ItemNudgeDropBehavior : StyledElementBehavior<ItemsControl>
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<ItemNudgeDropBehavior, Orientation>(nameof(Orientation),
            defaultValue: Orientation.Vertical);

    /// <summary>
    /// 
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AssociatedObject?.AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AssociatedObject?.AddHandler(DragDrop.DropEvent, OnDrop);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AssociatedObject?.RemoveHandler(DragDrop.DragOverEvent, OnDragOver);
        AssociatedObject?.RemoveHandler(DragDrop.DropEvent, OnDrop);
    }
    
    private void ApplyTranslation(Control control, double x, double y)
    {
        var transformBuilder = new TransformOperations.Builder(1);
        transformBuilder.AppendTranslate(x, y);
        control.RenderTransform = transformBuilder.Build();
    }
    
    private void RemoveTranslations(object? sender)
    {
        if (sender is ItemsControl itemsControl)
        {
            foreach (var container in itemsControl.GetRealizedContainers())
            {
                ApplyTranslation(container, 0, 0);
            }
        }
    }
    
    private void OnDrop(object? sender, DragEventArgs e)
    {
        RemoveTranslations(sender);
    }

    private void OnDragLeave(object? sender, RoutedEventArgs e)
    {
        RemoveTranslations(sender);
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (sender is not ItemsControl itemsControl) return;

        var isHorizontal = Orientation == Orientation.Horizontal;

        var dragPosition = e.GetPosition(itemsControl);

        Vector scrollOffset = Vector.Zero;
        
        if (itemsControl is ListBox {Scroll: { } scrollable})
        {
            scrollOffset = scrollable.Offset;
        }

        for (int index = 0; index < itemsControl.ItemCount; index++)
        {
            var container = itemsControl.ContainerFromIndex(index);

            if (container == null) continue;
            
            var containerMidPoint = isHorizontal ? container.Bounds.Center.X - scrollOffset.X : container.Bounds.Center.Y - scrollOffset.Y;
            
            var translationX = isHorizontal && dragPosition.X <= containerMidPoint ? container.Bounds.Width : 0;
            var translationY = !isHorizontal && dragPosition.Y <= containerMidPoint ? container.Bounds.Height : 0;

            ApplyTranslation(container, translationX, translationY);
        }
    }
}
