// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Events;

/// <summary>
/// Trigger that listens for the <see cref="InputElement.TextInputEvent"/>.
/// </summary>
public class TextInputEventTrigger : InteractiveTriggerBase
{
    static TextInputEventTrigger()
    {
        RoutingStrategiesProperty.OverrideMetadata<TextInputEventTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        Execute(e);
    }
}
