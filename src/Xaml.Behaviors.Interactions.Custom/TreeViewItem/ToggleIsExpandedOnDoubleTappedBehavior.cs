﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Toggles <see cref="TreeViewItem.IsExpanded"/> property of the associated <see cref="TreeViewItem"/> control on <see cref="InputElement.DoubleTapped"/> event.
/// </summary>
public class ToggleIsExpandedOnDoubleTappedBehavior : StyledElementBehavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.DoubleTapped += DoubleTapped;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.DoubleTapped -= DoubleTapped;
        }
    }

    private void DoubleTapped(object? sender, RoutedEventArgs args)
    {
        if (AssociatedObject is {Parent: TreeViewItem item})
        {
            item.IsExpanded = !item.IsExpanded;
        }
    }
}
