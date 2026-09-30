// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Microsoft.UI.Xaml;

namespace Xaml.Interactivity;

/// <summary>
/// Provides data for the action tree attachment notifications of <see cref="StyledElementAction"/>.
/// </summary>
/// <remarks>
/// WinUI has no logical tree. On Uno Platform actions form an action tree instead: an action is attached when the
/// trigger owning it (directly or through composite actions) is attached to an object, which becomes its host.
/// </remarks>
/// <param name="host">The object hosting the owning trigger.</param>
public sealed class LogicalTreeAttachmentEventArgs(DependencyObject host) : EventArgs
{
    /// <summary>
    /// Gets the object hosting the owning trigger.
    /// </summary>
    public DependencyObject Host { get; } = host;
}

/// <summary>
/// Uno Platform counterpart of the Avalonia logical tree membership check used by the shared sources.
/// </summary>
internal interface ILogical
{
    /// <summary>
    /// Gets a value indicating whether the element is attached to a (logical or action) tree.
    /// </summary>
    bool IsAttachedToLogicalTree { get; }
}
