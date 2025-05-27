// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class PinchEndedGestureTrigger : RoutedEventTriggerBase<PinchEndedEventArgs>
{
    /// <inheritdoc />
    protected override RoutedEvent<PinchEndedEventArgs> RoutedEvent
        => Gestures.PinchEndedEvent;

    static PinchEndedGestureTrigger()
    {
        EventRoutingStrategyProperty.OverrideMetadata<PinchEndedGestureTrigger>(
            new StyledPropertyMetadata<RoutingStrategies>(
                defaultValue: RoutingStrategies.Bubble));
    }
}
