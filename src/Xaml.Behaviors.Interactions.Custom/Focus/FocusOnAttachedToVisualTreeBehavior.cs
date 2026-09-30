// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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
