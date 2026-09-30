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
public abstract partial class StyledElementBehavior : Behavior
{
    /// <summary>
    /// Gets the <see cref="FrameworkElement"/> to which this behavior is attached.
    /// </summary>
    public FrameworkElement? AssociatedStyledElement => AssociatedObject as FrameworkElement;
}
