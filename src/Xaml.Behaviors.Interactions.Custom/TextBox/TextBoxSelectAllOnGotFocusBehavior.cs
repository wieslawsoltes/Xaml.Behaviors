﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows to select all <see cref="TextBox"/> text on got focus event.
/// </summary>
public class TextBoxSelectAllOnGotFocusBehavior : StyledElementBehavior<TextBox>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.GotFocusEvent, AssociatedObject_GotFocus, RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.GotFocusEvent, AssociatedObject_GotFocus);
    }

    private void AssociatedObject_GotFocus(object? sender, GotFocusEventArgs e)
    {
        AssociatedObject?.SelectAll();
    }
}
