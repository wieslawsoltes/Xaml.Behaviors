// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// Defines the mode of execution for <see cref="AsyncActionGroup"/>.
/// </summary>
public enum AsyncActionMode
{
    /// <summary>
    /// Actions are executed sequentially. If an action returns a Task, execution waits for it to complete before moving to the next action.
    /// </summary>
    Sequence,

    /// <summary>
    /// Actions are executed in parallel.
    /// </summary>
    Parallel
}

/// <summary>
/// An action that executes its child actions asynchronously, either in sequence or in parallel.
/// </summary>
public class AsyncActionGroup : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="Mode"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<AsyncActionMode> ModeProperty =
        AvaloniaProperty.Register<AsyncActionGroup, AsyncActionMode>(nameof(Mode));

    /// <summary>
    /// Identifies the <seealso cref="Actions"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ActionCollection?> ActionsProperty =
        AvaloniaProperty.Register<AsyncActionGroup, ActionCollection?>(nameof(Actions));

    /// <summary>
    /// Gets or sets the execution mode. This is an avalonia property.
    /// </summary>
    public AsyncActionMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of actions to execute. This is an avalonia property.
    /// </summary>
    [Content]
    public ActionCollection? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncActionGroup"/> class.
    /// </summary>
    public AsyncActionGroup()
    {
        SetCurrentValue(ActionsProperty, new ActionCollection());
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ActionsProperty)
        {
            var oldActions = change.GetOldValue<ActionCollection?>();
            var newActions = change.GetNewValue<ActionCollection?>();

            if (oldActions is not null && ((ILogical)this).IsAttachedToLogicalTree)
            {
                DetachActionsFromLogicalTree(oldActions);
            }

            if (newActions is not null && ((ILogical)this).IsAttachedToLogicalTree)
            {
                AttachActionsToLogicalTree(newActions);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        AttachActionsToLogicalTree(Actions);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        DetachActionsFromLogicalTree(Actions);
        base.OnDetachedFromLogicalTree(e);
    }

    private void AttachActionsToLogicalTree(ActionCollection? actions)
    {
        if (actions is null)
        {
            return;
        }

        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.AttachActionToLogicalTree(this);
            }
        }
    }

    private void DetachActionsFromLogicalTree(ActionCollection? actions)
    {
        if (actions is null)
        {
            return;
        }

        foreach (var action in actions)
        {
            if (action is StyledElementAction styledElementAction)
            {
                styledElementAction.DetachActionFromLogicalTree(this);
            }
        }
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (Actions == null)
        {
            return null;
        }

        // We fire and forget the async operation because Execute must return synchronously.
        // If the caller wants to await this, they would need this method to return a Task.
        // Since IAction.Execute returns object?, we can return the Task.
        return ExecuteAsync(sender, parameter);
    }

    private async Task ExecuteAsync(object? sender, object? parameter)
    {
        if (Actions == null)
        {
            return;
        }

        if (Mode == AsyncActionMode.Sequence)
        {
            foreach (var item in Actions)
            {
                if (item is IAction action)
                {
                    var result = action.Execute(sender, parameter);
                    if (result is Task task)
                    {
                        await task;
                    }
                }
            }
        }
        else // Parallel
        {
            var tasks = new List<Task>();
            foreach (var item in Actions)
            {
                if (item is IAction action)
                {
                    var result = action.Execute(sender, parameter);
                    if (result is Task task)
                    {
                        tasks.Add(task);
                    }
                }
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
        }
    }
}
