// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// A base class for actions that need data context aware plumbing.
/// </summary>
/// <remarks>
/// On Avalonia this type is a <c>StyledElement</c> that joins the logical tree. On Uno Platform actions inherit
/// the data context through the action collection, so this type shares the <see cref="Action"/> plumbing and
/// exists to keep the shared actions source compatible.
/// </remarks>
public abstract partial class StyledElementAction : Action, ILogical
{
    bool ILogical.IsAttachedToLogicalTree => Host is not null;

    /// <summary>
    /// Called after the action was attached to the action tree of an object (Avalonia: the logical tree).
    /// </summary>
    /// <param name="e">The attachment details.</param>
    protected virtual void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
    }

    /// <summary>
    /// Called before the action is detached from the action tree of an object (Avalonia: the logical tree).
    /// </summary>
    /// <param name="e">The attachment details.</param>
    protected virtual void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
    }

    /// <summary>
    /// Attaches a nested action (owned by a composite action) to the host of its parent.
    /// </summary>
    /// <param name="parent">The composite action or behavior owning this action.</param>
    internal void AttachActionToLogicalTree(DependencyObject parent)
    {
        var host = parent switch
        {
            Action action => action.Host,
            IBehavior behavior => behavior.AssociatedObject,
            IActionHostProvider provider => provider.ActionHost,
            _ => parent,
        };

        if (host is not null)
        {
            AttachToHost(host);
        }
    }

    /// <summary>
    /// Detaches a nested action from the host of its parent.
    /// </summary>
    /// <param name="parent">The composite action or behavior owning this action.</param>
    internal void DetachActionFromLogicalTree(DependencyObject parent)
    {
        _ = parent;
        DetachFromHost();
    }

    private protected override void OnHostAttached(DependencyObject host)
        => OnAttachedToLogicalTree(new LogicalTreeAttachmentEventArgs(host));

    private protected override void OnHostDetaching(DependencyObject host)
        => OnDetachedFromLogicalTree(new LogicalTreeAttachmentEventArgs(host));
}

/// <summary>
/// Notifies actions when their owning trigger is attached to or detached from an object.
/// </summary>
internal interface IActionLogicalTreeLifecycle
{
    void AttachedToActionLogicalTree();

    void DetachedFromActionLogicalTree();
}
