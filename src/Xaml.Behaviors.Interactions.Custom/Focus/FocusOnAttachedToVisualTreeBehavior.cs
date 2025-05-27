// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Focuses the <see cref="IBehavior.AssociatedObject"/> when attached to visual tree.
/// </summary>
public class FocusOnAttachedToVisualTreeBehavior : StyledElementBehavior<Control>
{
    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree()
    {
        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus());
    }
}
