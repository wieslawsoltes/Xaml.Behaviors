// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerReleasedTrigger : RoutedEventTriggerBase<PointerReleasedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerReleasedEventArgs> RoutedEvent 
        => InputElement.PointerReleasedEvent;

    static PointerReleasedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerReleasedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }
}
