// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Base class for behaviors that respond to drag-and-drop events.
/// </summary>
public abstract partial class DragAndDropEventsBehavior : StyledElementBehavior<Control>
{
    
    /// <summary>
    /// Gets or sets the control that is used as source for drag and drop events. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }
    
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        var targetControl = TargetControl ?? AssociatedObject;
        if (targetControl is null)
        {
            return;
        }

        AttachEvents(targetControl);
    }

    private void AttachEvents(Control targetControl)
    {
        DragDrop.SetAllowDrop(targetControl, true);
        targetControl.AddHandler(DragDrop.DragEnterEvent, DragEnter);
        targetControl.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        targetControl.AddHandler(DragDrop.DragOverEvent, DragOver);
        targetControl.AddHandler(DragDrop.DropEvent, Drop);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        var targetControl = TargetControl ?? AssociatedObject;
        if (targetControl is null)
        {
            return;
        }

        DetachEvents(targetControl);
    }

    private void DetachEvents(Control targetControl)
    {
        DragDrop.SetAllowDrop(targetControl, false);
        targetControl.RemoveRoutedEventHandler(DragDrop.DragEnterEvent, DragEnter);
        targetControl.RemoveRoutedEventHandler(DragDrop.DragLeaveEvent, DragLeave);
        targetControl.RemoveRoutedEventHandler(DragDrop.DragOverEvent, DragOver);
        targetControl.RemoveRoutedEventHandler(DragDrop.DropEvent, Drop);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TargetControlProperty)
        {
            var oldValue = change.GetOldValue<Control?>();
            if (oldValue is not null)
            {
                DetachEvents(oldValue);
            }

            var newValue = change.GetNewValue<Control?>();
            if (newValue is not null)
            {
                AttachEvents(newValue);
            }
        }
    }

    private void DragEnter(object? sender, DragEventArgs e)
    {
        OnDragEnter(sender, e);
    }

    private void DragLeave(object? sender, DragEventArgs e)
    {
        OnDragLeave(sender, e);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        OnDragOver(sender, e);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        OnDrop(sender, e);
    }

    /// <summary>
    /// Called when a drag-and-drop operation enters the element.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnDragEnter(object? sender, DragEventArgs e)
    {
    }

    /// <summary>
    /// Called when a drag-and-drop operation leaves the element.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnDragLeave(object? sender, DragEventArgs e)
    {
    }

    /// <summary>
    /// Called when a drag-and-drop operation is updated while over the element.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnDragOver(object? sender, DragEventArgs e)
    {
    }

    /// <summary>
    /// Called when a drag-and-drop operation should complete over the element.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnDrop(object? sender, DragEventArgs e)
    {
    }
}
