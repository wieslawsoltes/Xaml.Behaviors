﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Provides helper methods for custom drop handlers.
/// </summary>
public abstract class DropHandlerBase : IDropHandler
{
    /// <summary>
    /// Moves an item within a collection.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="sourceIndex"></param>
    /// <param name="targetIndex"></param>
    /// <typeparam name="T"></typeparam>
    protected void MoveItem<T>(IList<T> items, int sourceIndex, int targetIndex)
    {
        if (sourceIndex < targetIndex)
        {
            var item = items[sourceIndex];
            items.RemoveAt(sourceIndex);
            items.Insert(targetIndex, item);
        }
        else
        {
            var removeIndex = sourceIndex + 1;
            if (items.Count + 1 > removeIndex)
            {
                var item = items[sourceIndex];
                items.RemoveAt(removeIndex - 1);
                items.Insert(targetIndex, item);
            }
        }
    }

    /// <summary>
    /// Moves an item from one collection to another.
    /// </summary>
    /// <param name="sourceItems"></param>
    /// <param name="targetItems"></param>
    /// <param name="sourceIndex"></param>
    /// <param name="targetIndex"></param>
    /// <typeparam name="T"></typeparam>
    protected void MoveItem<T>(IList<T> sourceItems, IList<T> targetItems, int sourceIndex, int targetIndex)
    {
        var item = sourceItems[sourceIndex];
        sourceItems.RemoveAt(sourceIndex);
        targetItems.Insert(targetIndex, item);
    }
        
    /// <summary>
    /// Swaps two items inside a collection.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="sourceIndex"></param>
    /// <param name="targetIndex"></param>
    /// <typeparam name="T"></typeparam>
    protected void SwapItem<T>(IList<T> items, int sourceIndex, int targetIndex)
    {
        var item1 = items[sourceIndex];
        var item2 = items[targetIndex];
        items[targetIndex] = item1;
        items[sourceIndex] = item2;
    }

    /// <summary>
    /// Inserts an item into a collection at the specified index.
    /// </summary>
    /// <param name="sourceItems"></param>
    /// <param name="targetItems"></param>
    /// <param name="sourceIndex"></param>
    /// <param name="targetIndex"></param>
    /// <typeparam name="T"></typeparam>
    protected void SwapItem<T>(IList<T> sourceItems, IList<T> targetItems, int sourceIndex, int targetIndex)
    {
        var item1 = sourceItems[sourceIndex];
        var item2 = targetItems[targetIndex];
        targetItems[targetIndex] = item1;
        sourceItems[sourceIndex] = item2;
    }
        
    /// <summary>
    /// Inserts an item into a collection at the specified index.
    /// </summary>
    /// <param name="items">Collection to insert into.</param>
    /// <param name="item">Item being inserted.</param>
    /// <param name="index">Target index for insertion.</param>
    /// <typeparam name="T">The item type.</typeparam>
    protected void InsertItem<T>(IList<T> items, T item, int index)
    {
        items.Insert(index, item);
    }

    /// <summary>
    /// Called when a drag enters the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    public virtual void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (Validate(sender, e, sourceContext, targetContext, null) == false)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
        else
        {
            e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            e.Handled = true;
        }
    }

    /// <summary>
    /// Called continuously while a drag is over the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    public virtual void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (Validate(sender, e, sourceContext, targetContext, null) == false)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
        else
        {
            e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            e.Handled = true;
        }
    }

    /// <summary>
    /// Called when the drag is dropped on the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    public virtual async void Drop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (await ExecuteAsync(sender, e, sourceContext, targetContext, null) == false)
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
        }
        else
        {
            e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            e.Handled = true;
        }
    }

    /// <summary>
    /// Called when the drag leaves the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void Leave(object? sender, RoutedEventArgs e)
    {
        Cancel(sender, e);
    }

    /// <summary>
    /// Validates whether the drag operation can be performed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public virtual bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return false;
    }

    /// <summary>
    /// Executes the drop operation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public virtual bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return false;
    }

    /// <summary>
    /// Executes the drop operation asynchronously.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public virtual Task<bool> ExecuteAsync(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return Task.FromResult(Execute(sender, e, sourceContext, targetContext, state));
    }

    /// <summary>
    /// Cancels the drag operation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public virtual void Cancel(object? sender, RoutedEventArgs e)
    {
    }
}
