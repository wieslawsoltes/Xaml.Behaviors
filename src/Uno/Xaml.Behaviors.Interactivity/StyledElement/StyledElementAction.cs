// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Xaml.Interactivity;

/// <summary>
/// A base class for actions that need data context aware plumbing.
/// </summary>
/// <remarks>
/// On Avalonia this type is a <c>StyledElement</c> that joins the logical tree. On Uno Platform actions inherit
/// the data context through the action collection, so this type shares the <see cref="Action"/> plumbing and
/// exists to keep the shared actions source compatible.
/// </remarks>
public abstract partial class StyledElementAction : Action
{
}

/// <summary>
/// Notifies actions when their owning trigger is attached to or detached from an object.
/// </summary>
internal interface IActionLogicalTreeLifecycle
{
    void AttachedToActionLogicalTree();

    void DetachedFromActionLogicalTree();
}
