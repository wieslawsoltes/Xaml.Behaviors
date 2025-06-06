﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that displays cursor position on <see cref="InputElement.PointerMoved"/> event for the <see cref="StyledElementBehavior{T}.AssociatedObject"/> using <see cref="TextBlock.Text"/> property.
/// </summary>
public class ShowPointerPositionBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <seealso cref="TargetTextBlockProperty"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<TextBlock?> TargetTextBlockProperty =
        AvaloniaProperty.Register<ShowPointerPositionBehavior, TextBlock?>(nameof(TargetTextBlock));

    /// <summary>
    /// Gets or sets the target TextBlock object in which this behavior displays cursor position on PointerMoved event.
    /// </summary>
    [ResolveByName]
    public TextBlock? TargetTextBlock
    {
        get => GetValue(TargetTextBlockProperty);
        set => SetValue(TargetTextBlockProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved += AssociatedObject_PointerMoved;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved -= AssociatedObject_PointerMoved;
        }
    }

    private void AssociatedObject_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (TargetTextBlock is not null)
        {
            TargetTextBlock.Text = e.GetPosition(AssociatedObject).ToString();
        }
    }
}
