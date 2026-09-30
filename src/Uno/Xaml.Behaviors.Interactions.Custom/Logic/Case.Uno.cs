// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Microsoft.UI.Xaml;

namespace Xaml.Interactions.Custom;

/// <summary>
/// Uno Platform counterpart of the Avalonia <c>ISetLogicalParent</c> used by the switch behaviors to attach their
/// cases.
/// </summary>
internal interface ISetLogicalParent
{
    /// <summary>
    /// Sets the parent (the switch action or behavior), or <c>null</c> to detach.
    /// </summary>
    /// <param name="parent">The parent.</param>
    void SetParent(DependencyObject? parent);
}

/// <content>
/// Uno Platform action tree plumbing: WinUI has no logical tree, so a case joins the action tree of its switch.
/// </content>
public partial class Case : ILogical, ISetLogicalParent, IActionHostProvider
{
    private DependencyObject? _parent;

    bool ILogical.IsAttachedToLogicalTree => _parent is not null;

    /// <summary>
    /// Gets the object hosting the switch that owns this case (the host of its actions).
    /// </summary>
    DependencyObject? IActionHostProvider.ActionHost => _parent switch
    {
        Action action => action.Host,
        IBehavior behavior => behavior.AssociatedObject,
        _ => null,
    };

    void ISetLogicalParent.SetParent(DependencyObject? parent)
    {
        if (ReferenceEquals(_parent, parent))
        {
            return;
        }

        if (_parent is not null)
        {
            OnDetachedFromLogicalTree();
        }

        _parent = parent;

        if (_parent is not null)
        {
            OnAttachedToLogicalTree();
        }
    }
}
