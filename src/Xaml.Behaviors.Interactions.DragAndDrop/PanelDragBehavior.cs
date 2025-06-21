// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that starts a drag operation for a control so it can be moved between panels.
/// The control itself is used as the drag context.
/// </summary>
public sealed class PanelDragBehavior : ContextDragBehaviorBase
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        Context = AssociatedObject;
    }

    /// <inheritdoc />
    protected override void OnBeforeDragDrop(object? sender, PointerEventArgs e, object? context)
    {
    }

    /// <inheritdoc />
    protected override void OnAfterDragDrop(object? sender, PointerEventArgs e, object? context)
    {
    }
}
