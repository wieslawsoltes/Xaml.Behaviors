// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Defines callbacks for handling drag-and-drop operations.
/// </summary>
public interface IDropHandler
{
    /// <summary>
    /// Called when a drag enters the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    void Enter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);

    /// <summary>
    /// Called repeatedly as the drag moves over the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);

    /// <summary>
    /// Called when the drag is dropped on the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    void Drop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext);

    /// <summary>
    /// Called when the drag leaves the target.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Leave(object? sender, RoutedEventArgs e);

    /// <summary>
    /// Validates whether the drag can be executed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state);

    /// <summary>
    /// Executes the drop logic.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="sourceContext"></param>
    /// <param name="targetContext"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state);

    /// <summary>
    /// Cancels the drag operation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Cancel(object? sender, RoutedEventArgs e);
}
