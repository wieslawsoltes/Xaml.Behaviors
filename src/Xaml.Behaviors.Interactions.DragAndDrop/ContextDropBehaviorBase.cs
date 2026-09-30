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
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Behavior base class that enables dropping context data onto the associated control.
/// </summary>
public abstract partial class ContextDropBehaviorBase : StyledElementBehavior<Control>
{
#if UNO
    /// <summary>
    /// Identifies the application data format used to store context identifiers (a <c>DataPackage</c> format identifier).
    /// </summary>
    public static readonly string ContextDataTransferFormat = "Avalonia.Xaml.Interactions.DragAndDrop.Context";
#else
    /// <summary>
    /// Identifies the application data format used to store context identifiers.
    /// </summary>
    public static readonly DataFormat<string> ContextDataTransferFormat =
        DataFormat.CreateStringApplicationFormat("Avalonia.Xaml.Interactions.DragAndDrop.Context");
#endif

    /// <summary>
    /// Gets or sets context data provided to the drop handler.
    /// </summary>
    [StyledProperty]
    public partial object? Context { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            DragDrop.SetAllowDrop(AssociatedObject, true);
        }
        AssociatedObject?.AddHandler(DragDrop.DragEnterEvent, DragEnter);
        AssociatedObject?.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        AssociatedObject?.AddHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject?.AddHandler(DragDrop.DropEvent, Drop);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            DragDrop.SetAllowDrop(AssociatedObject, false);
        }
        AssociatedObject?.RemoveRoutedEventHandler(DragDrop.DragEnterEvent, DragEnter);
        AssociatedObject?.RemoveRoutedEventHandler(DragDrop.DragLeaveEvent, DragLeave);
        AssociatedObject?.RemoveRoutedEventHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject?.RemoveRoutedEventHandler(DragDrop.DropEvent, Drop);
    }

    /// <summary>
    /// Called when a drag enters the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    protected abstract void OnEnter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);
    
    /// <summary>
    /// Called when the drag leaves the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected abstract void OnLeave(object? sender, RoutedEventArgs e);

    /// <summary>
    /// Called repeatedly as the drag moves over the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    protected abstract void OnOver(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);

    /// <summary>
    /// Called when the drag is dropped on the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    protected abstract void OnDrop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);

    private void DragEnter(object? sender, DragEventArgs e)
    {
        var sourceContextKey = e.DataTransfer.TryGetValue(ContextDataTransferFormat);
        var sourceContext = DragDropContextStore.Get(sourceContextKey);
        var targetContext = Context ?? AssociatedObject?.DataContext;
        OnEnter(sender, e, sourceContext, targetContext);
    }

    private void DragLeave(object? sender, RoutedEventArgs e)
    {
        OnLeave(sender, e);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        var sourceContextKey = e.DataTransfer.TryGetValue(ContextDataTransferFormat);
        var sourceContext = DragDropContextStore.Get(sourceContextKey);
        var targetContext = Context ?? AssociatedObject?.DataContext;
        OnOver(sender, e, sourceContext, targetContext);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var sourceContextKey = e.DataTransfer.TryGetValue(ContextDataTransferFormat);
        var sourceContext = DragDropContextStore.Get(sourceContextKey);
        var targetContext = Context ?? AssociatedObject?.DataContext;
        OnDrop(sender, e, sourceContext, targetContext);
    }
}
