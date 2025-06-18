﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Behavior that handles the <see cref="InputElement.TextInputMethodClientRequestedEvent"/>.
/// </summary>
public abstract class TextInputMethodClientRequestedEventBehavior : InteractiveBehaviorBase
{
    static TextInputMethodClientRequestedEventBehavior()
    {
        RoutingStrategiesProperty.OverrideMetadata<TextInputMethodClientRequestedEventBehavior>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.TextInputMethodClientRequestedEvent, TextInputMethodClientRequested, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.TextInputMethodClientRequestedEvent, TextInputMethodClientRequested);
    }

    private void TextInputMethodClientRequested(object? sender, TextInputMethodClientRequestedEventArgs e)
    {
        OnTextInputMethodClientRequested(sender, e);
    }

    /// <summary>
    /// Called when a text input method client is requested for the associated control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnTextInputMethodClientRequested(object? sender, TextInputMethodClientRequestedEventArgs e)
    {
    }
}
