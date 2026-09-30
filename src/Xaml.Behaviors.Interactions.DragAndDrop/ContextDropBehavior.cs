// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
using Avalonia.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Behavior that enables dropping context data onto the associated control using predefined <see cref="IDropHandler"/>.
/// </summary>
public partial class ContextDropBehavior : ContextDropBehaviorBase
{

    /// <summary>
    /// Gets or sets the drop handler that receives drop notifications.
    /// </summary>
    [StyledProperty]
    public partial IDropHandler? Handler { get; set; }

    /// <inheritdoc />
    protected override void OnEnter(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        Handler?.Enter(sender, e, sourceContext, targetContext);
    }

    /// <inheritdoc />
    protected override void OnLeave(object? sender, RoutedEventArgs e)
    {
        Handler?.Leave(sender, e);
    }

    /// <inheritdoc />
    protected override void OnOver(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        Handler?.Over(sender, e, sourceContext, targetContext);
    }

    /// <inheritdoc />
    protected override void OnDrop(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        Handler?.Drop(sender, e, sourceContext, targetContext);
    }
}
