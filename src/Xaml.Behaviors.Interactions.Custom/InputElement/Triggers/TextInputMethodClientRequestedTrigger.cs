// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class TextInputMethodClientRequestedTrigger : RoutedEventTriggerBase<TextInputMethodClientRequestedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TextInputMethodClientRequestedEventArgs> RoutedEvent 
        => InputElement.TextInputMethodClientRequestedEvent;

    static TextInputMethodClientRequestedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TextInputMethodClientRequestedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }
}
