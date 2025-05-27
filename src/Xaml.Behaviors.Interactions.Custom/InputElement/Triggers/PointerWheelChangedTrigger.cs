// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PointerWheelChangedTrigger : RoutedEventTriggerBase<PointerWheelEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PointerWheelEventArgs> RoutedEvent 
        => InputElement.PointerWheelChangedEvent;

    static PointerWheelChangedTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PointerWheelChangedTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Tunnel | RoutingStrategies.Bubble));
    }
}
