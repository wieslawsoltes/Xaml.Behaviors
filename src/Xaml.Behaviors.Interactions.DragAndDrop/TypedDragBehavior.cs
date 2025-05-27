// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;

namespace Avalonia.Xaml.Interactions.DragAndDrop;

/// <summary>
/// Behavior that initiates a drag operation for a specific data type.
/// </summary>
public class TypedDragBehavior : TypedDragBehaviorBase
{
    /// <summary>
    /// Identifies the <see cref="Handler"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<IDragHandler?> HandlerProperty =
        AvaloniaProperty.Register<TypedDragBehavior, IDragHandler?>(nameof(Handler));

    /// <summary>
    /// Gets or sets the handler that receives drag notifications.
    /// </summary>
    public IDragHandler? Handler
    {
        get => GetValue(HandlerProperty);
        set => SetValue(HandlerProperty, value);
    }

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
