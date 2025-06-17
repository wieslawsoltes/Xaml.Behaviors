// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Trigger that listens for the <see cref="TextBox.TextChangedEvent"/>.
/// </summary>
public class TextChangedTrigger : RoutedEventTriggerBase<TextChangedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<TextChangedEventArgs> RoutedEvent
        => TextBox.TextChangedEvent;

    static TextChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<TextChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
