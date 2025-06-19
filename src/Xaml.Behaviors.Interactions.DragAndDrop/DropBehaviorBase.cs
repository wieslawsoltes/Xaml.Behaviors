// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Base class for behaviors that respond to drag-and-drop events.
/// </summary>
public abstract class DropBehaviorBase : InvokeCommandBehaviorBase
{
    /// <summary>
    /// Gets or sets the handler responsible for processing drop events.
    /// </summary>
    protected IDropHandler? Handler { get; set; }

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
        AssociatedObject?.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
        AssociatedObject?.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
        AssociatedObject?.RemoveHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject?.RemoveHandler(DragDrop.DropEvent, Drop);
    }

    private void DragEnter(object? sender, DragEventArgs e)
    {
        Handler?.Enter(sender, e, null, null);
    }

    private void DragLeave(object? sender, RoutedEventArgs e)
    {
        Handler?.Leave(sender, e);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        Handler?.Over(sender, e, null, null);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        Handler?.Drop(sender, e, null, null);
    }

    /// <summary>
    /// Executes the associated <see cref="System.Windows.Input.ICommand"/>.
    /// </summary>
    /// <param name="parameter"></param>
    protected void ExecuteCommand(object? parameter)
    {
        if (IsEnabled != true || Command is null)
        {
            return;
        }

        if (AssociatedObject is not { IsVisible: true, IsEnabled: true })
        {
            return;
        }

        var resolvedParameter = ResolveParameter(parameter);

        if (Command?.CanExecute(resolvedParameter) != true)
        {
            return;
        }

        Command.Execute(resolvedParameter);
    }
}
