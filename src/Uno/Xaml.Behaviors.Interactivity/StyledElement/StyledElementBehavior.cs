// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// A base class for behaviors that need data context and template aware plumbing.
/// </summary>
/// <remarks>
/// On Avalonia this type is a <c>StyledElement</c> that joins the logical tree. WinUI has no logical tree;
/// on Uno Platform behaviors inherit the data context through the behavior collection, so this type shares
/// the <see cref="Behavior"/> plumbing and exists to keep the shared behaviors source compatible.
/// </remarks>
public abstract partial class StyledElementBehavior : Behavior, ILogical
{
    private bool _isAttachedToLogicalTree;

    bool ILogical.IsAttachedToLogicalTree => _isAttachedToLogicalTree;

    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree()
    {
        base.OnAttachedToLogicalTree();

        if (AssociatedObject is { } host && !_isAttachedToLogicalTree)
        {
            _isAttachedToLogicalTree = true;
            OnAttachedToLogicalTree(new LogicalTreeAttachmentEventArgs(host));
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree()
    {
        if (AssociatedObject is { } host && _isAttachedToLogicalTree)
        {
            _isAttachedToLogicalTree = false;
            OnDetachedFromLogicalTree(new LogicalTreeAttachmentEventArgs(host));
        }

        base.OnDetachedFromLogicalTree();
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (_isAttachedToLogicalTree && AssociatedObject is { } host)
        {
            _isAttachedToLogicalTree = false;
            OnDetachedFromLogicalTree(new LogicalTreeAttachmentEventArgs(host));
        }

        base.OnDetaching();
    }

    /// <summary>
    /// Called after the behavior joined the tree of its associated object (Avalonia: the logical tree).
    /// </summary>
    /// <param name="e">The attachment details.</param>
    protected virtual void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
    }

    /// <summary>
    /// Called before the behavior leaves the tree of its associated object (Avalonia: the logical tree).
    /// </summary>
    /// <param name="e">The attachment details.</param>
    protected virtual void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
    }

    /// <summary>
    /// Gets the <see cref="FrameworkElement"/> to which this behavior is attached.
    /// </summary>
    public FrameworkElement? AssociatedStyledElement => AssociatedObject as FrameworkElement;
}
