// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
#else
using Avalonia.Input;
#endif

#if UNO
namespace Xaml.Interactions.DragAndDrop;
#else
namespace Avalonia.Xaml.Interactions.DragAndDrop;
#endif

/// <summary>
/// Behavior that starts a drag operation using the associated context data.
/// </summary>
public partial class ContextDragBehavior : ContextDragBehaviorBase
{

    /// <summary>
    /// Gets or sets the handler that receives drag notifications.
    /// </summary>
    [StyledProperty]
    public partial IDragHandler? Handler { get; set; }

    /// <inheritdoc />
    protected override void OnBeforeDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        Handler?.BeforeDragDrop(sender, e, context);
    }

    /// <inheritdoc />
    protected override void OnAfterDragDrop(object? sender, PointerEventArgs e, object? context)
    {
        Handler?.AfterDragDrop(sender, e, context);
    }
}
